using System;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Interfaces.Files;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.EditorScope
{
    public sealed class PreloadedAsset : ILoadableAsset
    {
        private readonly Type _assetType;
        private readonly ILoadableAsset _asset;

        public PreloadedAsset(ILoadableAsset asset, Type assetType)
        {
            _asset = asset;
            _assetType = assetType;
        }

        public string Name => _asset.Name;

        public string Path => _asset.Path;

        public bool IsLoaded => _asset.IsLoaded;

        public Object Load()
        {
#if UNITY_EDITOR
            if (PreloadedAssetLoader.TryLoadAsset(_assetType, out var asset) ||
                AssetDatabaseLoader.TryLoadAsset(_assetType, out asset))
            {
                return asset;
            }
#endif

            return _asset.Load();
        }

        public void Unload() => _asset.Unload();

        public void LoadAsync(IAssetLoadingCallbacks callbacks)
        {
#if UNITY_EDITOR
            if (PreloadedAssetLoader.TryLoadAsset(_assetType, out var asset) ||
                AssetDatabaseLoader.TryLoadAsset(_assetType, out asset))
            {
                callbacks.InvokeProgressEvent(1f);
                callbacks.InvokeLoadedEvent(asset);
                return;
            }
#endif

            _asset.LoadAsync(callbacks);
        }
    }
}