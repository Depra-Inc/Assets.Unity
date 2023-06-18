// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Delegates;
using Depra.Assets.Runtime.Files.Exceptions;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.ValueObjects;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Group
{
    public sealed class AssetGroup :
        ILoadableAsset<IEnumerable<Object>>,
        IEnumerable<ILoadableAsset<Object>>,
        IDisposable
    {
        private readonly NamedAssetIdent _ident;
        private readonly List<Object> _loadedAssets;
        private readonly List<ILoadableAsset<Object>> _childAssets;

        public AssetGroup(NamedAssetIdent ident, List<ILoadableAsset<Object>> children = null)
        {
            _ident = ident;
            _childAssets = children ?? new List<ILoadableAsset<Object>>();
            _loadedAssets = new List<Object>(_childAssets.Count);
        }

        public IAssetIdent Ident => _ident;
        public bool IsLoaded => _childAssets.All(asset => asset.IsLoaded);
        public FileSize Size => new(_childAssets.Sum(x => x.Size.SizeInBytes));

        public void AddAsset(ILoadableAsset<Object> asset)
        {
            Guard.AgainstNull(asset, () => new ArgumentNullException(nameof(asset)));
            Guard.AgainstAlreadyContains(asset, _childAssets,
                () => new AssetAlreadyAddedToGroup(asset.Ident.RelativeUri));

            _childAssets.Add(asset);
        }

        public IEnumerable<Object> Load()
        {
            foreach (var asset in _childAssets)
            {
                if (asset.IsLoaded)
                {
                    yield return asset.Load();
                }

                var loadedAsset = asset.Load();
                Guard.AgainstNull(loadedAsset, () => new AssetGroupLoadingException(_ident.Name));
                Guard.AgainstAlreadyContains(loadedAsset, @in: _loadedAssets,
                    () => new AssetAlreadyLoadedException(loadedAsset.name));

                _loadedAssets.Add(loadedAsset);

                yield return loadedAsset;
            }
        }

        public async UniTask<IEnumerable<Object>> LoadAsync(DownloadProgressDelegate onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);

                return _loadedAssets;
            }

            await UniTask.WhenAll(TasksCompleted());
            OnProgressChanged();

            return _loadedAssets;

            IEnumerable<UniTask> TasksCompleted() =>
                _childAssets.Select(asset => LoadAssetAsync(asset, cancellationToken));

            async UniTask LoadAssetAsync(ILoadableAsset<Object> asset, CancellationToken token)
            {
                var loadedAsset = await asset.LoadAsync(cancellationToken: token);
                OnProgressChanged();

                Guard.AgainstNull(loadedAsset, () => new AssetGroupLoadingException(_ident.Name));
                Guard.AgainstAlreadyContains(loadedAsset, @in: _loadedAssets,
                    () => new AssetAlreadyLoadedException(loadedAsset.name));

                _loadedAssets.Add(loadedAsset);
            }

            void OnProgressChanged()
            {
                var progressValue = (float) _loadedAssets.Count / _childAssets.Count;
                var progress = new DownloadProgress(progressValue);
                onProgress?.Invoke(progress);
            }
        }

        public void Unload()
        {
            _loadedAssets.Clear();
            foreach (var asset in _childAssets)
            {
                asset.Unload();
            }
        }

        public IEnumerator<ILoadableAsset<Object>> GetEnumerator() => _childAssets.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IDisposable.Dispose() => Unload();
    }
}