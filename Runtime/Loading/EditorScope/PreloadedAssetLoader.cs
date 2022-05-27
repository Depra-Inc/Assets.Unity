#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.EditorScope
{
    public static class PreloadedAssetLoader
    {
        public static bool TryLoadAsset<T>(out T asset) where T : Object
        {
            asset = null;
            var assetType = typeof(T);

            if (TryLoadAsset(assetType, out var @object))
            {
                asset = @object as T;
                return asset != null;
            }

            return false;
        }

        public static bool TryLoadAsset(Type assetType, out Object preloadedAsset)
        {
            preloadedAsset = GetPreloaded(assetType);
            return preloadedAsset != null;
        }

        private static Object GetPreloaded(Type assetType)
        {
            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            return preloadedAssets.FirstOrDefault(asset => asset.GetType() == assetType);
        }
    }
}

#endif