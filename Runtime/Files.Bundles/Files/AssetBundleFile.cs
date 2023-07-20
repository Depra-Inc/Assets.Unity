// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Idents;
using Depra.Assets.Unity.Runtime.Files.Bundles.Extensions;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Adapter;
using Depra.Assets.Unity.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Idents;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Files
{
    public abstract class AssetBundleFile : IUnityLoadableAsset<AssetBundle>, IDisposable
    {
        public static implicit operator AssetBundle(AssetBundleFile from) => from.Load();
        
        private readonly FileSystemAssetIdent _ident;
        private AssetBundle _loadedAssetBundle;

        protected AssetBundleFile(FileSystemAssetIdent ident) =>
            _ident = ident ?? throw new ArgumentNullException(nameof(ident));

        public IAssetIdent Ident => _ident;
        public bool IsLoaded => _loadedAssetBundle != null;
        public FileSize Size { get; private set; } = FileSize.Unknown;

        public AssetBundle Load()
        {
            if (IsLoaded)
            {
                return _loadedAssetBundle;
            }

            var loadedAssetBundle = LoadOverride();
            Guard.AgainstNull(loadedAssetBundle, () => new AssetBundleNotLoadedException(Ident.Uri));
            
            _loadedAssetBundle = loadedAssetBundle;
            Size = FindSize(_loadedAssetBundle);

            return _loadedAssetBundle;
        }

        public async UniTask<AssetBundle> LoadAsync(DownloadProgressDelegate onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);
                return _loadedAssetBundle;
            }

            var progress = Progress.Create<float>(value => onProgress?.Invoke(new DownloadProgress(value)));
            var loadedAssetBundle = await LoadAsyncOverride(progress, cancellationToken);
            Guard.AgainstNull(loadedAssetBundle, () => new AssetBundleNotLoadedException(Ident.Uri));
            
            _loadedAssetBundle = loadedAssetBundle;
            onProgress?.Invoke(DownloadProgress.Full);
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

        protected abstract UniTask<AssetBundle> LoadAsyncOverride(IProgress<float> progress = null,
            CancellationToken cancellationToken = default);

        protected virtual FileSize FindSize(AssetBundle assetBundle) =>
            assetBundle.Size();

        void IDisposable.Dispose() => Unload();
    }
}