using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Files.Structs;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Profiling;
using UnityEditor;
#endif

namespace Depra.Assets.Runtime.Files.Bundles.Extensions
{
    public static class AssetBundleExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "JoinDeclarationAndInitializer")]
        public static FileSize Size(this AssetBundle assetBundle)
        {
            FileSize fileSize;
#if UNITY_EDITOR
            fileSize = SizeInMemory(assetBundle);
            if (fileSize.SizeInBytes == 0)
#endif
            {
                fileSize = SizeOnDisk(assetBundle);
            }

            return fileSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FileSize SizeOnDisk(this AssetBundle assetBundle)
        {
            long bytes = 0;
            var allScenePaths = assetBundle.GetAllScenePaths();
            foreach (var scenePath in allScenePaths)
            {
                var absolutePath = Path.Combine(Application.streamingAssetsPath, scenePath);
                var fileInfo = new FileInfo(absolutePath);
                if (fileInfo.Exists)
                {
                    bytes += fileInfo.Length;
                }
            }

            return new FileSize(bytes);
        }

#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FileSize SizeInMemory(this Object assetBundle)
        {
            long bytes = 0;
            var assets = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundle.name);
            foreach (var path in assets)
            {
                var fileInfo = new FileInfo(path);
                if (fileInfo.Exists == false)
                {
                    continue;
                }

                var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
                bytes = Profiler.GetRuntimeMemorySizeLong(mainAsset);
                // The above isn't supported for all asset types:
                if (bytes == 0)
                {
                    bytes = fileInfo.Length;
                }
            }

            return new FileSize(bytes);
        }
#endif
    }
}