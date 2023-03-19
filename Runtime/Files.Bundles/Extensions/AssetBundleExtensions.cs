using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Extensions
{
    public static class AssetBundleExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FileSize Size(this AssetBundle assetBundle)
        {
            long bytes = 0;
            var allScenePaths = assetBundle.GetAllScenePaths();
            foreach (var scenePath in allScenePaths)
            {
                var absolutePath = Path.Combine(Application.streamingAssetsPath, scenePath);
                var fileInfo = new FileInfo(absolutePath);
                bytes += fileInfo.Length;
            }

            return new FileSize(bytes);
        }
    }
}