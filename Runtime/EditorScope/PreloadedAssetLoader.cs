#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.EditorScope
{
    public static class PreloadedAssetLoader
    {
        public static bool TryLoadAsset<T>(out T asset) where T : Object
        {
            asset = null;
            var assetType = typeof(T);

            if (TryLoadAsset(assetType, out var @object))
            {
                return @object as T != null;
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