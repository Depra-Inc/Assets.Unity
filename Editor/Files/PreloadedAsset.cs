// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Delegates;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.ValueObjects;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Depra.Assets.Editor.Files
{
    public sealed class PreloadedAsset<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
    {
        public static implicit operator TAsset(PreloadedAsset<TAsset> from) => from.Load();

        private static Type AssetType => typeof(TAsset);

        private readonly ILoadableAsset<TAsset> _asset;
        private TAsset _loadedAsset;

        public PreloadedAsset(ILoadableAsset<TAsset> asset) =>
            _asset = asset ?? throw new ArgumentNullException(nameof(asset));

        public IAssetIdent Ident => _asset.Ident;
        public bool IsLoaded => _loadedAsset != null;
        public FileSize Size { get; private set; } = FileSize.Unknown;

        public TAsset Load()
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
            Size = FileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        public void Unload()
        {
            if (IsLoaded == false)
            {
                return;
            }

            _asset.Unload();
            _loadedAsset = null;
        }

        public async UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
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
            Size = FileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        private bool TryGetPreloadedAsset(out TAsset preloadedAsset)
        {
            bool Filter(Object asset) => asset.GetType() == AssetType;
            var assetByType = PlayerSettings
                .GetPreloadedAssets()
                .FirstOrDefault(Filter);

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
            var filter = $"t:{AssetType.Name}";
            var assetGuid = AssetDatabase
                .FindAssets(filter)
                .FirstOrDefault();

            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            asset = AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);

            return asset != null;
        }

        void IDisposable.Dispose() => Unload();
    }
}