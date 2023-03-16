using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Tests.PlayMode.Utils;
using Depra.Coroutines.Unity.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode
{
    [TestFixture(TestOf = typeof(AssetBundleFile))]
    internal sealed class LoadingAssetBundles
    {
        private static RuntimeCoroutineHost _coroutineHost;
        private Stopwatch _stopwatch;

        private static IEnumerable<AssetBundleFile> AllBundles() => 
            Load.AllBundles(_coroutineHost);
        
        private static IEnumerator Free(AssetBundle assetBundle)
        {
            assetBundle.Unload(true);
            yield return null;
        }

        [OneTimeSetUp]
        public void OneTimeSetup() => 
            _coroutineHost = new GameObject().AddComponent<RuntimeCoroutineHost>();

        [SetUp]
        public void Setup() => _stopwatch = new Stopwatch();

        [OneTimeTearDown]
        public void OneTimeTearDown() => 
            Object.Destroy(_coroutineHost.gameObject);

        [UnityTest]
        public IEnumerator AssetBundleShouldBeLoaded(
            [ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.

            // Act.
            var loadedAssetBundle = assetBundleFile.Load();

            // Assert.
            Assert.IsNotNull(loadedAssetBundle);
            Assert.IsTrue(assetBundleFile.IsLoaded);
            
            // Debug.
            Debug.Log($"Loaded bundle [{loadedAssetBundle.name}] by path: [{assetBundleFile.Path}].");
            
            yield return Free(loadedAssetBundle);
        }

        [UnityTest]
        public IEnumerator AssetBundleShouldBeUnloaded(
            [ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.
            assetBundleFile.Load();
            yield return null;

            // Act.
            assetBundleFile.Unload();
            yield return null;

            // Assert.
            Assert.IsFalse(assetBundleFile.IsLoaded);
            
            // Debug.
            Debug.Log($"Loaded and unloaded bundle [{assetBundleFile.Name}] by path: [{assetBundleFile.Path}].");
        }

        [UnityTest]
        public IEnumerator AssetBundleShouldBeLoadedAsync(
            [ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
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

            // Assert.
            Assert.NotNull(loadedAssetBundle);
            Assert.IsTrue(assetBundleFile.IsLoaded);
            
            // Debug.
            Debug.Log($"Loaded bundle [{loadedAssetBundle.name}] " +
                      $"by path: [{assetBundleFile.Path}] " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");
            
            yield return Free(loadedAssetBundle);
        }
    }
}