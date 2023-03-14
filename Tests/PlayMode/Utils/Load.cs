using System.Collections.Generic;
using System.IO;
using Depra.Assets.Runtime.Bundle.Files;
using Depra.Assets.Runtime.Bundle.Files.IO;
using Depra.Assets.Runtime.Bundle.Files.Memory;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Tests.PlayMode.Configuration;
using Depra.Coroutines.Domain.Entities;
using Depra.Coroutines.Unity.Runtime;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Utils
{
    internal static class Load
    {
        public static IEnumerable<AssetIdent> AssetIdents(string assetDirectoryName)
        {
            var assetNames = AssetDirectory(assetDirectoryName).Assets;
            if (assetNames == null || assetNames.Length == 0)
            {
                throw new AssetsTestException(assetDirectoryName);
            }

            foreach (var assetName in assetNames)
            {
                yield return new AssetIdent(assetName, assetDirectoryName);
            }
        }

        public static IEnumerable<AssetBundleFile> AllBundles(string assetDirectoryName)
        {
            var assetDirectory = TestAssetBundleReference.Load(assetDirectoryName);
            var absoluteDirectoryPath = Path.Combine(Application.streamingAssetsPath, assetDirectory.Path);
            ICoroutineHost coroutineHost = new GameObject().AddComponent<RuntimeCoroutineHost>();

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