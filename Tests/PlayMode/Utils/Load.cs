using System.Collections.Generic;
using System.Linq;
using Depra.Assets.Runtime.Bundle.Files;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Bundles.IO;
using Depra.Assets.Runtime.Files.Bundles.Memory;
using Depra.Assets.Tests.PlayMode.Exceptions;

namespace Depra.Assets.Tests.PlayMode.Utils
{
    internal static class Load
    {
        public static IEnumerable<AssetIdent> AssetIdents()
        {
            var assetBundle = AssetBundle();
            var assetNames = assetBundle.Assets;
            if (assetNames == null || assetNames.Length == 0)
            {
                throw new TestAssetsConfigurationException(assetBundle.BundleName);
            }

            foreach (var assetName in assetNames)
            {
                yield return new AssetIdent(assetName, assetBundle.Path);
            }
        }

        public static IEnumerable<AssetBundleFile> AllBundles()
        {
            var assetBundle = AssetBundle();
            var coroutineHost = Create.RuntimeCoroutineHost();
            var bundleIdent = new AssetIdent(assetBundle.BundleName, assetBundle.AbsoluteDirectoryPath);

            yield return new AssetBundleFromFile(bundleIdent, coroutineHost);
            yield return new AssetBundleFromMemory(bundleIdent, coroutineHost);
            yield return new AssetBundleFromStream(bundleIdent, coroutineHost);
            //yield return new AssetBundleFromWeb(bundleIdent, coroutineHost);
        }

        public static TestResourcesRef Resources() =>
            UnityEngine.Resources.LoadAll<TestResourcesRef>(string.Empty).FirstOrDefault() ??
            throw new TestReferenceNotFoundException(nameof(TestResourcesRef));

        private static TestAssetBundleRef AssetBundle() =>
            UnityEngine.Resources.LoadAll<TestAssetBundleRef>(string.Empty).FirstOrDefault() ??
            throw new TestReferenceNotFoundException(nameof(TestAssetBundleRef));
    }
}