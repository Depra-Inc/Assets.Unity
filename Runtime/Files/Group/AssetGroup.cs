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
using Depra.Assets.Runtime.Files.Extensions;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.ValueObjects;
using JetBrains.Annotations;
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
        private readonly List<ILoadableAsset<Object>> _children;

        public AssetGroup(NamedAssetIdent ident, List<ILoadableAsset<Object>> children = null)
        {
            _ident = ident ?? throw new InvalidOperationException(nameof(ident));
            _children = children ?? new List<ILoadableAsset<Object>>();
            _loadedAssets = new List<Object>(_children.Count);
            Size = _children.SizeForAll();
        }

        public IAssetIdent Ident => _ident;
        public FileSize Size { get; private set; }
        public bool IsLoaded => Children.All(asset => asset.IsLoaded);

        [UsedImplicitly]
        public IEnumerable<ILoadableAsset<Object>> Children => _children;

        public void AddAsset(ILoadableAsset<Object> asset)
        {
            Guard.AgainstNull(asset, () => new ArgumentNullException(nameof(asset)));
            Guard.AgainstAlreadyContains(asset, _children,
                () => new AssetAlreadyAddedToGroup(asset.Ident.RelativeUri));

            _children.Add(asset);
        }

        public IEnumerable<Object> Load()
        {
            foreach (var asset in Children)
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

            Size = Children.SizeForAll();
        }

        public async UniTask<IEnumerable<Object>> LoadAsync(DownloadProgressDelegate onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);

                return _loadedAssets;
            }

            await UniTask.WhenAll(Children.Select(asset => LoadAssetAsync(asset, cancellationToken)));
            OnProgressChanged();
            Size = Children.SizeForAll();

            return _loadedAssets;

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
                var progressValue = (float) _loadedAssets.Count / _children.Count;
                var progress = new DownloadProgress(progressValue);
                onProgress?.Invoke(progress);
            }
        }

        public void Unload()
        {
            _loadedAssets.Clear();
            foreach (var asset in Children)
            {
                asset.Unload();
            }
        }

        public IEnumerator<ILoadableAsset<Object>> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IDisposable.Dispose()
        {
            Unload();
            _children.Clear();
        }
    }
}