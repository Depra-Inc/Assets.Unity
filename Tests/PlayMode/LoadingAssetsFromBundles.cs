using System.Collections;
using System.Diagnostics;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Bundle.Files;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Tests.PlayMode.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode
{
    [TestFixture(TestOf = typeof(AssetBundleFile))]
    internal sealed class LoadingAssetsFromBundles
    {
        private Stopwatch _stopwatch;

        private static IEnumerator Free(AssetBundle assetBundle)
        {
            assetBundle.Unload(true);
            yield return null;
        }

        [SetUp]
        public void Setup() => _stopwatch = new Stopwatch();

        public void TearDown()
        {
            
        }

        [UnityTest]
        public IEnumerator AssetBundleShouldBeLoaded(
            [ValueSource(typeof(Load), nameof(Load.AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.

            // Act.
            var loadedAssetBundle = assetBundleFile.Load();
            Debug.Log($"Loaded bundle [{assetBundleFile.Name}] by path: [{assetBundleFile.Path}].");

            // Assert.
            Assert.IsNotNull(loadedAssetBundle);
            Assert.IsTrue(assetBundleFile.IsLoaded);
            
            yield return Free(loadedAssetBundle);
        }

        [UnityTest]
        public IEnumerator AssetBundleShouldBeUnloaded(
            [ValueSource(typeof(Load), nameof(Load.AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.
            assetBundleFile.Load();
            yield return null;

            // Act.
            assetBundleFile.Unload();
            yield return null;
            Debug.Log($"Loaded and unloaded bundle [{assetBundleFile.Name}] by path: [{assetBundleFile.Path}].");

            // Assert.
            Assert.IsFalse(assetBundleFile.IsLoaded);
        }

        [UnityTest]
        public IEnumerator AssetBundleShouldBeLoadedAsync(
            [ValueSource(typeof(Load), nameof(Load.AllBundles))]
            AssetBundleFile assetBundleFile)
        {
            // Arrange.
            AssetBundle loadedAssetBundle = null;
            var assetLoadingCallbacks = new AssetLoadingCallbacks<AssetBundle>(
                onLoaded: asset => loadedAssetBundle = asset,
                onFailed: exception => throw exception);

            // Act.
            _stopwatch.Restart();
            assetBundleFile.LoadAsync(assetLoadingCallbacks);
            while (loadedAssetBundle == null)
            {
                yield return null;
            }

            _stopwatch.Stop();
            Debug.Log($"Loaded bundle [{assetBundleFile.Name}] " +
                      $"by path: [{assetBundleFile.Path}] " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

            // Assert.
            Assert.NotNull(loadedAssetBundle);
            Assert.IsTrue(assetBundleFile.IsLoaded);
            
            yield return Free(loadedAssetBundle);
        }
    }
}