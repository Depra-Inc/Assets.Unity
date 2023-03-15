using System.Collections;
using System.Diagnostics;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Bundle.Files;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Tests.Common;
using Depra.Assets.Tests.PlayMode.Utils;
using NUnit.Framework;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode
{
    [TestFixture(TestOf = typeof(AssetBundleAssetFile<>))]
    internal sealed class LoadingAssetBundles
    {
        private Stopwatch _stopwatch;
        private AssetIdent _assetIdent;

        private static IEnumerator Free(AssetBundleFile assetBundleFile)
        {
            assetBundleFile.Unload();
            yield return null;
        }

        [SetUp]
        public void Setup() => _stopwatch = new Stopwatch();

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeLoaded(
            [ValueSource(typeof(Load), nameof(Load.AssetIdents))]
            AssetIdent assetIdent,
            [ValueSource(typeof(Load), nameof(Load.AllBundles))]
            AssetBundleFile assetBundleFile)
        {
            // Arrange.
            var bundleAsset = new AssetBundleAssetFile<TestAsset>(assetIdent, assetBundleFile);

            // Act.
            var loadedAsset = bundleAsset.Load();
            Debug.Log($"Loaded [{bundleAsset.Name}] from bundle {assetBundleFile.Name}.");

            // Assert.
            Assert.IsNotNull(loadedAsset);
            Assert.IsTrue(bundleAsset.IsLoaded);

            yield return Free(assetBundleFile);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeUnloaded(
            [ValueSource(typeof(Load), nameof(Load.AssetIdents))]
            AssetIdent assetIdent,
            [ValueSource(typeof(Load), nameof(Load.AllBundles))]
            AssetBundleFile assetBundleFile)
        {
            // Arrange.
            var bundleAsset = new AssetBundleAssetFile<TestAsset>(assetIdent, assetBundleFile);
            bundleAsset.Load();
            yield return null;

            // Act.
            bundleAsset.Unload();
            yield return null;
            Debug.Log($"Loaded and unloaded [{bundleAsset.Name}] from bundle [{assetBundleFile.Name}].");

            // Assert.
            Assert.IsFalse(bundleAsset.IsLoaded);

            yield return Free(assetBundleFile);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeLoadedAsync(
            [ValueSource(typeof(Load), nameof(Load.AssetIdents))]
            AssetIdent assetIdent,
            [ValueSource(typeof(Load), nameof(Load.AllBundles))]
            AssetBundleFile assetBundleFile)
        {
            // Arrange.
            TestAsset loadedAsset = null;
            var assetLoadingCallbacks = new AssetLoadingCallbacks<TestAsset>(
                onLoaded: asset => loadedAsset = asset,
                onFailed: exception => throw exception);

            // Act.
            _stopwatch.Restart();
            assetBundleFile.LoadAsync(assetIdent.Name, assetLoadingCallbacks);
            while (loadedAsset == null)
            {
                yield return null;
            }

            _stopwatch.Stop();
            Debug.Log($"Loaded [{assetIdent.Name}] " +
                      $"from bundle [{assetBundleFile.Name}] " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

            // Assert.
            Assert.NotNull(loadedAsset);

            yield return Free(assetBundleFile);
        }
    }
}