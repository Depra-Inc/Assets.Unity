using System.Collections.Generic;
using Depra.Assets.Runtime.Bundle.Files;
using Depra.Assets.Runtime.Bundle.Files.IO;
using Depra.Assets.Runtime.Bundle.Files.Memory;
using Depra.Assets.Runtime.Common;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Configuration
{
    internal static class AssetBundlesTestUtils
    {
        public static IEnumerable<AssetBundleFile> CreateAssetBundles(TestAssetDirectory assetDirectory,
            ICoroutineHost coroutineHost)
        {
            var absoluteDirectoryPath = System.IO.Path.Combine(Application.streamingAssetsPath, assetDirectory.Path);
            foreach (var bundleName in assetDirectory.Assets)
            {
                var bundleIdent = new AssetIdent(bundleName, absoluteDirectoryPath);

                yield return new AssetBundleFromFile(bundleIdent, coroutineHost);
                yield return new AssetBundleFromMemory(bundleIdent, coroutineHost);
                yield return new AssetBundleFromStream(bundleIdent, coroutineHost);
                //yield return new AssetBundleFromWeb(bundleIdent, coroutineHost);
            }
        }
    }
}