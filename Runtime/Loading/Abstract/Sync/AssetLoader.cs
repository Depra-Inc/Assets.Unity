using System;
using Depra.Assets.Runtime.Utils;
using Depra.Assets.Runtime.Loading.Abstract.Utils;
using Depra.Assets.Runtime.Loading.EditorScope;
using Depra.Assets.Runtime.Loading.Interfaces.Sync;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.Abstract.Sync
{
    public abstract class AssetLoader : IAssetLoader
    {
        public abstract Object LoadAsset(Type assetType, string assetPath);

        public abstract T LoadAsset<T>(string assetPath) where T : Object;

        public abstract void UnloadAsset(Object asset);

        protected virtual Object LoadAsset(Type assetType)
        {
            return default;
        }

        public T LoadAsset<T>() where T : Object
        {
            T assetAsT;
            var assetType = typeof(T);

#if UNITY_EDITOR
            // In editor, the player settings preloaded assets work differently, mainly they aren't loaded.
            if (PreloadedAssetLoader.TryLoadAsset(out assetAsT))
            {
                return assetAsT;
            }

            if (AssetDatabaseLoader.TryLoadAsset(out assetAsT))
            {
                return assetAsT;
            }
#endif

            var asset = LoadAsset(assetType);
            if (asset == null)
            {
                throw new AssetLoadingException(assetType);
            }

            assetAsT = asset as T;
            if (asset == null)
            {
                throw new AssetLoadingException(assetType);
            }

            return assetAsT;
        }
    }
}