#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Loading.EditorScope
{
    public static class AssetDatabaseLoader
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

        public static bool TryLoadAsset(Type assetType, out Object asset)
        {
            var assetGuid = AssetDatabase.FindAssets($"t:{assetType.Name}").FirstOrDefault();
            asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGuid), assetType);

            return asset != null;
        }
    }
}

#endif