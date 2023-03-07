using System.Collections.Generic;
using System.IO;
using Depra.Assets.Runtime.Bundle.Files;
using Depra.Assets.Runtime.Bundle.Strategies.IO;
using Depra.Assets.Runtime.Bundle.Strategies.Memory;
using Depra.Assets.Runtime.Common;
using Depra.Coroutines.Editor;
using UnityEngine;

namespace Depra.Assets.Tests.EditMode.Bundle
{
    internal static class AssetBundlesTestUtils
    {
        private const string ASSET_BUNDLE_NAME = "test";

        private static readonly string ASSET_BUNDLES_DIRECTORY =
            Path.Combine(Application.streamingAssetsPath, "AssetBundle", "Windows");

        public static IEnumerable<AssetBundleFile> AssetBundleLoaders()
        {
            var ident = new AssetIdent(ASSET_BUNDLE_NAME, ASSET_BUNDLES_DIRECTORY);
            var coroutineHost = new EditorCoroutineHost();
            
            yield return new AssetBundleFile(ident, new AssetBundleFromFile(coroutineHost));
            yield return new AssetBundleFile(ident, new AssetBundleFromMemory(coroutineHost));
            yield return new AssetBundleFile(ident, new AssetBundleFromStream(coroutineHost));
        }

        public static void PrintAllAssetNames(AssetBundleFile assetBundle)
        {
            var assetNames = assetBundle.AllAssetNames;
            foreach (var assetName in assetNames)
            {
                Debug.Log(assetName);
            }
        }
    }
}