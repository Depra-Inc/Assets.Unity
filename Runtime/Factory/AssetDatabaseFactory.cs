#if UNITY_EDITOR

using System.IO;
using Depra.Assets.Runtime.Common;
using UnityEditor;
using UnityEngine;
using static Depra.Assets.Runtime.Common.Static;

namespace Depra.Assets.Runtime.Factory
{
    internal static class AssetDatabaseFactory
    {
        public static Object Create(Object asset, string directory, string assetName,
            string typeExtension = AssetTypes.Base)
        {
            var absolutePath = CombineIntoPath(Application.dataPath, directory);
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

        public static void Delete(Object asset)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            Delete(assetPath);
        }

        public static void Delete(string path) => AssetDatabase.DeleteAsset(path);
        
        private static string MakeAssetPath(string directory, string assetName, string type)
        {
            var assetPath = Path.Combine(AssetsConstants.ASSETS_FOLDER_NAME, directory, assetName);
            assetPath += type;
            return assetPath;
        }
    }
}

#endif