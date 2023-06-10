// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Extensions;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Resource;
using Depra.Assets.Runtime.Files.Structs;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Files
{
    public abstract class AssetBundleFile : ILoadableAsset<AssetBundle>, IDisposable
    {
        private readonly FileSystemAssetIdent _ident;
        private AssetBundle _loadedAssetBundle;

        protected AssetBundleFile(FileSystemAssetIdent ident) =>
            _ident = ident ?? throw new ArgumentNullException(nameof(ident));

        public string Name => _ident.Name;
        public string Path => _ident.Path;

        public bool IsLoaded => _loadedAssetBundle != null;
        public FileSize Size { get; private set; } = FileSize.Unknown;

        public AssetBundle Load()
        {
            if (IsLoaded)
            {
                return _loadedAssetBundle;
            }

            var loadedAssetBundle = LoadOverride();
            Guard.AgainstNull(loadedAssetBundle, () => new AssetBundleNotLoadedException(Path));
            _loadedAssetBundle = loadedAssetBundle;
            Size = FindSize(_loadedAssetBundle);

            return _loadedAssetBundle;
        }

        public async UniTask<AssetBundle> LoadAsync(CancellationToken cancellationToken,
            DownloadProgressDelegate onProgress = null)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);

                return _loadedAssetBundle;
            }

            var progress = Progress.Create<float>(value => onProgress?.Invoke(new DownloadProgress(value)));
            var loadedAssetBundle = await LoadAsyncOverride(cancellationToken, progress);
            onProgress?.Invoke(DownloadProgress.Full);
            
            Guard.AgainstNull(loadedAssetBundle, () => new AssetBundleNotLoadedException(Path));
            _loadedAssetBundle = loadedAssetBundle;
            Size = FindSize(_loadedAssetBundle);

            return _loadedAssetBundle;
        }

        public void Unload()
        {
            if (IsLoaded == false)
            {
                return;
            }

            _loadedAssetBundle.Unload(true);
            _loadedAssetBundle = null;
        }

        public void UnloadAsync()
        {
            if (IsLoaded == false)
            {
                return;
            }

            _loadedAssetBundle.UnloadAsync(true);
            _loadedAssetBundle = null;
        }

        protected abstract AssetBundle LoadOverride();

        protected abstract UniTask<AssetBundle> LoadAsyncOverride(CancellationToken cancellationToken,
            IProgress<float> progress = null);

        protected virtual FileSize FindSize(AssetBundle assetBundle) =>
            assetBundle.Size();

        void IDisposable.Dispose() => Unload();
    }
}