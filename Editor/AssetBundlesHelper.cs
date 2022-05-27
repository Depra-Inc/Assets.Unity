using System.IO;
using UnityEditor;
using UnityEngine;

namespace Depra.Assets.Editor
{
    internal static class AssetBundlesHelper
    {
        private const string MenuDirectory = "Assets/Asset Bundles";

        private const BuildAssetBundleOptions PcBuildOptions = BuildAssetBundleOptions.None;
        private const BuildAssetBundleOptions AndroidBuildOptions = BuildAssetBundleOptions.None;
        private const BuildAssetBundleOptions IOSBuildOptions = BuildAssetBundleOptions.None;

        [MenuItem(MenuDirectory + "/Clear Asset Bundle Cache")]
        private static void ClearAssetBundleCache()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            Debug.Log($"Clear Asset Bundle Cache result: {Caching.ClearCache()}");
        }

        [MenuItem(MenuDirectory + "/Create PC Asset Bundle")]
        private static void CreatePCBundle()
        {
            const string bundlePath = "Assets/StreamingAssets/AssetBundle";

            CreateDirectoryIfMissing(bundlePath);

            BuildPipeline.BuildAssetBundles(bundlePath, PcBuildOptions, BuildTarget.StandaloneOSX);
        }

        [MenuItem(MenuDirectory + "/Create Android Asset Bundle")]
        private static void CreateAndroidBundle()
        {
            const string bundlePath = "Assets/StreamingAssets/AssetBundle/Android";

            CreateDirectoryIfMissing(bundlePath);

            BuildPipeline.BuildAssetBundles(bundlePath, AndroidBuildOptions, BuildTarget.Android);
        }

        [MenuItem(MenuDirectory + "/Create IOS Asset Bundle")]
        private static void CreateIOSBundle()
        {
            const string bundlePath = "Assets/StreamingAssets/AssetBundle/IOS";
            
            CreateDirectoryIfMissing(bundlePath);
            
            BuildPipeline.BuildAssetBundles(bundlePath, IOSBuildOptions, BuildTarget.iOS);
        }

        private static void CreateDirectoryIfMissing(string directoryPath)
        {
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}