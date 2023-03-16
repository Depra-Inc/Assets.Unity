using System.Collections.Generic;
using System.Linq;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Bundles.IO;
using Depra.Assets.Runtime.Files.Bundles.Memory;
using Depra.Assets.Tests.PlayMode.Exceptions;
using Depra.Assets.Tests.PlayMode.Types;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Utils
{
    internal static class Load
    {
        public static IEnumerable<AssetBundleFile> AllBundles(ICoroutineHost coroutineHost)
        {
            var assetBundle = AssetBundle();
            var bundleIdent = new AssetIdent(assetBundle.BundleName, assetBundle.AbsoluteDirectoryPath);

            yield return new AssetBundleFromFile(bundleIdent, coroutineHost);
            yield return new AssetBundleFromMemory(bundleIdent, coroutineHost);
            yield return new AssetBundleFromStream(bundleIdent, coroutineHost);
            //yield return new AssetBundleFromWeb(bundleIdent, coroutineHost);
        }

        public static TestAssetBundleRef AssetBundle() =>
            Resources.LoadAll<TestAssetBundleRef>(string.Empty).FirstOrDefault() ??
            throw new TestReferenceNotFoundException(nameof(TestAssetBundleRef));
    }
}