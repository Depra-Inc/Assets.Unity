// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Idents;
using Depra.Assets.Unity.Runtime.Common;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Adapter;
using Depra.Assets.Unity.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.ValueObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Files
{
    public sealed class AssetBundleAssetFile<TAsset> : UnityAssetFile<TAsset>, IDisposable where TAsset : Object
    {
        public static implicit operator TAsset(AssetBundleAssetFile<TAsset> from) => from.Load();

        private readonly string _assetName;
        private readonly AssetBundle _assetBundle;
        private readonly AssetBundleAssetFileIdent _ident;

        private TAsset _loadedAsset;

        public AssetBundleAssetFile(string assetName, AssetBundle assetBundle)
        {
            _ident = new AssetBundleAssetFileIdent(assetName ?? throw new ArgumentNullException(nameof(assetName)));
            _assetBundle = assetBundle ? assetBundle : throw new ArgumentNullException(nameof(assetBundle));
        }

        public override IAssetIdent Ident => _ident;
        public override bool IsLoaded => _loadedAsset != null;
        public override FileSize Size { get; protected set; } = FileSize.Unknown;

        public override TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var loadedAsset = _assetBundle.LoadAsset<TAsset>(_ident.Name);
            Guard.AgainstNull(loadedAsset,
                () => new AssetBundleFileNotLoadedException(_ident.Name, _assetBundle.name));

            _loadedAsset = loadedAsset;
            Size = UnityFileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        public override void Unload()
        {
            if (IsLoaded)
            {
                _loadedAsset = null;
            }
        }

        public override async UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);
                return _loadedAsset;
            }

            var request = _assetBundle.LoadAssetAsync<TAsset>(_ident.Name);
            while (request.isDone == false)
            {
                var progress = new DownloadProgress(request.progress);
                onProgress?.Invoke(progress);

                await UniTask.Yield();
            }

            Guard.AgainstNull(request.asset,
                () => new AssetBundleFileNotLoadedException(_ident.Name, _assetBundle.name));

            _loadedAsset = (TAsset) request.asset;
            onProgress?.Invoke(DownloadProgress.Full);
            Size = UnityFileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        void IDisposable.Dispose() => Unload();

        private readonly struct AssetBundleAssetFileIdent : IAssetIdent
        {
            public readonly string Name;

            public AssetBundleAssetFileIdent(string name) => Name = name;

            string IAssetIdent.Uri => Name;
            string IAssetIdent.RelativeUri => Name;
        }
    }
}