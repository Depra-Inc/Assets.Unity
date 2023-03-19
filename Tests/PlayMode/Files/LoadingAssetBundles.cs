using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Files;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Tests.PlayMode.Types;
using Depra.Assets.Tests.PlayMode.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(AssetBundleFile))]
    internal sealed class LoadingAssetBundles
    {
        private static TestCoroutineHost _coroutineHost;
        private Stopwatch _stopwatch;

        private static TestCoroutineHost CoroutineHost => 
            _coroutineHost ??= TestCoroutineHost.Create();
        
        private static IEnumerable<AssetBundleFile> AllBundles() => 
            Load.AllBundles(CoroutineHost);
        
        private static IEnumerator Free(AssetBundle assetBundle)
        {
            assetBundle.Unload(true);
            yield return null;
        }

        [SetUp]
        public void Setup()
        {
            _stopwatch = new Stopwatch();
        }
        
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Object.DestroyImmediate(_coroutineHost.gameObject);
        }

        [UnityTest]
        public IEnumerator AssetBundleShouldBeLoaded(
            [ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.

            // Act.
            var loadedAssetBundle = assetBundleFile.Load();

            // Assert.
            Assert.That(loadedAssetBundle, Is.Not.Null);
            Assert.That(assetBundleFile.IsLoaded);
            
            // Debug.
            var assetSize = assetBundleFile.Size.ToHumanReadableString();
            Debug.Log($"Loaded bundle [{loadedAssetBundle.name} : {assetSize}] by path: [{assetBundleFile.Path}].");
            
            yield return Free(loadedAssetBundle);
        }

        [UnityTest]
        public IEnumerator AssetBundleShouldBeUnloaded(
            [ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.
            assetBundleFile.Load();
            var assetSize = assetBundleFile.Size.ToHumanReadableString();
            yield return null;

            // Act.
            assetBundleFile.Unload();
            yield return null;

            // Assert.
            Assert.That(assetBundleFile.IsLoaded, Is.False);
            
            // Debug.
            Debug.Log($"Loaded and unloaded bundle [{assetBundleFile.Name} : {assetSize}] " +
                      $"by path: [{assetBundleFile.Path}].");
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
            Assert.That(loadedAssetBundle, Is.Not.Null);
            Assert.That(assetBundleFile.IsLoaded);
            
            // Debug.
            Debug.Log($"Loaded bundle [{loadedAssetBundle.name}] " +
                      $"by path: [{assetBundleFile.Path}] " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.\n" +
                      $"Size: {assetBundleFile.Size.ToHumanReadableString()}");
            
            yield return Free(loadedAssetBundle);
        }
    }
}