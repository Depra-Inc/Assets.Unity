// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Idents;
using Depra.Assets.Unity.Runtime.Common;
using Depra.Assets.Unity.Runtime.Files.Adapter;
using Depra.Assets.ValueObjects;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Depra.Assets.Unity.Editor.Files
{
    public sealed class PreloadedAsset<TAsset> : UnityAssetFile<TAsset>, IDisposable where TAsset : Object
    {
        public static implicit operator TAsset(PreloadedAsset<TAsset> from) => from.Load();

        private static Type AssetType => typeof(TAsset);

        private readonly IUnityLoadableAsset<TAsset> _asset;
        private TAsset _loadedAsset;

        public PreloadedAsset(IUnityLoadableAsset<TAsset> asset) =>
            _asset = asset ?? throw new ArgumentNullException(nameof(asset));

        public override IAssetIdent Ident => _asset.Ident;
        public override bool IsLoaded => _loadedAsset != null;
        public override FileSize Size { get; protected set; } = FileSize.Unknown;

        public override TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            if (TryGetPreloadedAsset(out var loadedAsset) == false &&
                TryLoadAssetFromDatabase(out loadedAsset) == false)
            {
                loadedAsset = _asset.Load();
            }

            _loadedAsset = loadedAsset;
            Size = UnityFileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        public override void Unload()
        {
            if (IsLoaded == false)
            {
                return;
            }

            _asset.Unload();
            _loadedAsset = null;
        }

        public override async UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);
                return _loadedAsset;
            }

            if (TryGetPreloadedAsset(out var loadedAsset) == false &&
                TryLoadAssetFromDatabase(out loadedAsset) == false)
            {
                loadedAsset = await _asset.LoadAsync(onProgress, cancellationToken);
            }

            _loadedAsset = loadedAsset;
            Size = UnityFileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        private bool TryGetPreloadedAsset(out TAsset preloadedAsset)
        {
            var assetByType = PlayerSettings
                .GetPreloadedAssets()
                .FirstOrDefault(asset => asset.GetType() == AssetType);

            if (assetByType == null)
            {
                preloadedAsset = null;
                return false;
            }

            preloadedAsset = (TAsset) assetByType;
            return preloadedAsset != null;
        }

        private bool TryLoadAssetFromDatabase(out TAsset asset)
        {
            var assetGuid = AssetDatabase
                .FindAssets($"t:{AssetType.Name}")
                .FirstOrDefault();

            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            asset = AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);

            return asset != null;
        }

        void IDisposable.Dispose() => Unload();
    }
}