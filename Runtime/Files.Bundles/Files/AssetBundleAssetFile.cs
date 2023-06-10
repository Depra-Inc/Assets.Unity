﻿// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Resource;
using Depra.Assets.Runtime.Files.Structs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Bundles.Files
{
    public sealed class AssetBundleAssetFile<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
    {
        private readonly AssetBundle _assetBundle;
        private readonly FileSystemAssetIdent _ident;

        private TAsset _loadedAsset;

        public AssetBundleAssetFile(FileSystemAssetIdent ident, AssetBundle assetBundle)
        {
            _ident = ident ?? throw new ArgumentNullException(nameof(ident));
            _assetBundle = assetBundle ? assetBundle : throw new ArgumentNullException(nameof(assetBundle));
        }

        public string Name => _ident.Name;
        public string Path => _ident.Path;

        public bool IsLoaded => _loadedAsset != null;
        public FileSize Size { get; private set; } = FileSize.Unknown;

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var loadedAsset = _assetBundle.LoadAsset<TAsset>(Name);
            Guard.AgainstNull(loadedAsset, () => new AssetBundleFileNotLoadedException(Name, _assetBundle.name));

            _loadedAsset = loadedAsset;
            Size = FileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                _loadedAsset = null;
            }
        }

        public async UniTask<TAsset> LoadAsync(CancellationToken cancellationToken,
            DownloadProgressDelegate onProgress = null)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);

                return _loadedAsset;
            }

            var assetBundleRequest = _assetBundle.LoadAssetAsync<TAsset>(Name);
            while (assetBundleRequest.isDone == false)
            {
                var progress = new DownloadProgress(assetBundleRequest.progress);
                onProgress?.Invoke(progress);

                await UniTask.Yield();
            }

            onProgress?.Invoke(DownloadProgress.Full);

            Guard.AgainstNull(assetBundleRequest.asset,
                () => new AssetBundleFileNotLoadedException(Name, _assetBundle.name));

            var loadedAsset = (TAsset) assetBundleRequest.asset;
            _loadedAsset = loadedAsset;
            Size = FileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        void IDisposable.Dispose() => Unload();

        public static implicit operator TAsset(AssetBundleAssetFile<TAsset> assetFile) => assetFile.Load();
    }
}