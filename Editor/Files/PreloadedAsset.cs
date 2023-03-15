using System;
using System.Linq;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Files;
using Depra.Assets.Runtime.Internal.Patterns;
using UnityEditor;
using IDisposable = System.IDisposable;
using Object = UnityEngine.Object;

namespace Depra.Assets.Editor.Files
{
    public sealed class PreloadedAsset<TAsset> : ILoadableAsset<TAsset> where TAsset : Object
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

            return _loadedAsset = loadedAsset;
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

        public IDisposable LoadAsync(IAssetLoadingCallbacks<TAsset> callbacks)
        {
            if (IsLoaded)
            {
                return OnLoadedInstantly(_loadedAsset);
            }
            
            if (TryGetPreloadedAsset(out var asset) || TryLoadAssetFromDatabase(out asset))
            {
                return OnLoadedInstantly(asset);
            }

            return _asset.LoadAsync(callbacks);

            IDisposable OnLoadedInstantly(TAsset readyAsset)
            {
                callbacks.InvokeProgressEvent(1f);
                callbacks.InvokeLoadedEvent(readyAsset);
                return new EmptyDisposable();
            }
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

            preloadedAsset = (TAsset)assetByType;
            return preloadedAsset != null;
        }

        private bool TryLoadAssetFromDatabase(out TAsset asset)
        {
            const string filterFormat = "t:{0}";
            var filter = string.Format(filterFormat, _assetType.Name);
            var assetGuid = AssetDatabase.FindAssets(filter).FirstOrDefault();
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            asset = AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);

            return asset != null;
        }
    }
}