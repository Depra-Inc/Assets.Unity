#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Depra.Assets.Runtime.Factory
{
    internal static class AssetTypes
    {
        public const string Base = ".asset";
        public const string Material = ".material";
        public const string Sprite = ".sprite";
    }
    
    public static class AssetDatabaseFactory
    {
        public static Object CreateAsset(Object asset, string directory, string assetName,
            string typeExtension = AssetTypes.Base)
        {
            var absolutePath = MakeAbsolutePath(directory);
            if (Directory.Exists(absolutePath) == false)
            {
                Directory.CreateDirectory(absolutePath);
            }

            var fullPath = MakeAssetPath(directory, assetName, typeExtension);
            asset.name = assetName;

            AssetDatabase.CreateAsset(asset, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }

        public static void DeleteAsset(Object asset)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.DeleteAsset(assetPath);
        }

        private static string MakeAbsolutePath(string directory)
        {
            var absolutePath = Path.Combine(Application.dataPath, "Resources", directory);
            return absolutePath;
        }

        private static string MakeAssetPath(string directory, string assetName, string type)
        {
            var assetPath = Path.Combine("Assets", "Resources", directory, assetName, type);
            return assetPath;
        }
    }
}

#endif