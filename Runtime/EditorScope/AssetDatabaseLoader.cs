#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.EditorScope
{
    public static class AssetDatabaseLoader
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

        public static bool TryLoadAsset(Type assetType, out Object asset)
        {
            var filter = $"t:{assetType.Name}";
            var assetGuid = AssetDatabase.FindAssets(filter).FirstOrDefault();
            asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGuid), assetType);

            return asset != null;
        }
    }
}

#endif