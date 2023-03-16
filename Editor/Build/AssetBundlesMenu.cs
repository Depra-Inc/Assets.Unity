using System.IO;
using UnityEditor;
using UnityEngine;

namespace Depra.Assets.Editor.Build
{
    internal static class AssetBundlesMenu
    {
        private const string MENU_DIRECTORY = "Assets/Asset Bundles/";
        
        [MenuItem(MENU_DIRECTORY + "Clear Cache")]
        private static void ClearAssetBundleCache()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            var result = Caching.ClearCache();
            Debug.Log($"Clear Asset Bundle Cache result: {result}");
        }
        
        [MenuItem(MENU_DIRECTORY + "Get AssetBundle Names")]
        private static void GetNames() {
            var names = AssetDatabase.GetAllAssetBundleNames();
            foreach(var name in names){
                Debug.Log("AssetBundle name is : " + name);
            }
        }

        internal static class AssetBundlesBuildMenu
        {
            private const string MENU_NAME = "Build/";
            
            private const BuildAssetBundleOptions PC_BUILD_OPTIONS = BuildAssetBundleOptions.None;
            private const BuildAssetBundleOptions ANDROID_BUILD_OPTIONS = BuildAssetBundleOptions.None;
            private const BuildAssetBundleOptions IOS_BUILD_OPTIONS = BuildAssetBundleOptions.None;

            [MenuItem(MENU_DIRECTORY + MENU_NAME + "Current")]
            private static void Build()
            {
                AssetBundleBuilder.BuildAssetBundles();
            }
            
            [MenuItem(MENU_DIRECTORY + MENU_NAME + "Windows")]
            private static void BuildWindowsBundles()
            {
                const string bundlePath = "Assets/StreamingAssets/AssetBundle/Windows";
                BuildAssetBundles(bundlePath, PC_BUILD_OPTIONS, BuildTarget.StandaloneWindows);
            }
            
            [MenuItem(MENU_DIRECTORY + MENU_NAME + "Windows x 64")]
            private static void BuildWindows64Bundles()
            {
                const string bundlePath = "Assets/StreamingAssets/AssetBundle/Windows";
                BuildAssetBundles(bundlePath, PC_BUILD_OPTIONS, BuildTarget.StandaloneWindows64);
            }

            [MenuItem(MENU_DIRECTORY + MENU_NAME + "Android")]
            private static void BuildAndroidBundles()
            {
                const string bundlePath = "Assets/StreamingAssets/AssetBundle/Android";
                BuildAssetBundles(bundlePath, ANDROID_BUILD_OPTIONS, BuildTarget.Android);
            }

            [MenuItem(MENU_DIRECTORY + MENU_NAME + "IOS")]
            private static void BuildIOSBundles()
            {
                const string bundlePath = "Assets/StreamingAssets/AssetBundle/IOS";
                BuildAssetBundles(bundlePath, IOS_BUILD_OPTIONS, BuildTarget.iOS);
            }

            /// <summary>
            /// Used to create a menu item called Build AssetBundles to build asset bundles directly using this.
            /// </summary>
            /// <param name="bundlePath">Build directory.</param>
            /// <param name="options">Asset bundle build options.</param>
            /// <param name="target">Target platform.</param>
            private static void BuildAssetBundles(string bundlePath, BuildAssetBundleOptions options, BuildTarget target)
            {
                CreateDirectoryIfMissing(bundlePath);
                BuildPipeline.BuildAssetBundles(bundlePath, options, target);
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
}