using UnityEditor;
using UnityEngine;

namespace Depra.Assets.Unity.Tests.PlayMode.Utils
{
    public static class TestEnvironment
    {
        public static TAsset CreateAsset<TAsset>(string path) where TAsset : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<TAsset>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }

        public static bool TryDeleteAsset(Object asset)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            if (assetPath == null)
            {
                return false;
            }
            
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return true;
        }
    }
}