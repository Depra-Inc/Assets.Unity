// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Resource;
using Depra.Assets.Runtime.Files.Structs;
using UnityEditor;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Depra.Assets.Editor.Files
{
    public sealed class PreloadedAsset<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
    {
        private readonly Type _assetType;
        private readonly ILoadableAsset<TAsset> _asset;

        private TAsset _loadedAsset;

        public PreloadedAsset(ILoadableAsset<TAsset> asset)
        {
            _asset = asset;
            _assetType = typeof(TAsset);
        }

        public string Name => _asset.Name;
        public string Path => _asset.Path;

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
            Size = FindSize(_loadedAsset);

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

        public async UniTask<TAsset> LoadAsync(CancellationToken cancellationToken,
            DownloadProgressDelegate onProgress = null)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);

                return _loadedAsset;
            }

            if (TryGetPreloadedAsset(out var loadedAsset) == false &&
                TryLoadAssetFromDatabase(out loadedAsset) == false)
            {
                loadedAsset = await _asset.LoadAsync(cancellationToken, onProgress);
            }

            _loadedAsset = loadedAsset;
            Size = FindSize(_loadedAsset);

            return _loadedAsset;
        }

        private bool TryGetPreloadedAsset(out TAsset preloadedAsset)
        {
            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            var assetByType = preloadedAssets.FirstOrDefault(asset => asset.GetType() == _assetType);
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
            const string FILTER_FORMAT = "t:{0}";
            var filter = string.Format(FILTER_FORMAT, _assetType.Name);
            var assetGuid = AssetDatabase.FindAssets(filter).FirstOrDefault();
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            asset = AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);

            return asset != null;
        }

        private FileSize FindSize(Object asset) =>
            new(Profiler.GetRuntimeMemorySizeLong(asset));

        void IDisposable.Dispose() => Unload();
    }
}