using System.Collections;
using System.Diagnostics;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Tests.PlayMode.Types;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(AssetBundleAssetFile<>))]
    internal sealed class LoadingAssetsFromBundles
    {
        private Stopwatch _stopwatch;
        private AssetBundle _assetBundle;
        private TestCoroutineHost _coroutineHost;
        private ILoadableAsset<TestScriptableAsset> _assetFromBundle;

        private static IEnumerator Free(AssetBundle assetBundle)
        {
            assetBundle.Unload(true);
            yield return null;
        }

        [SetUp]
        public void Setup()
        {
            _stopwatch = new Stopwatch();
            _coroutineHost = TestCoroutineHost.Create();
            var assetBundleReference = TestAssetBundleRef.Load();
            _assetBundle = AssetBundle.LoadFromFile(assetBundleReference.AbsolutePath);
            var assetIdent = new AssetIdent(assetBundleReference.SingleAssetName, assetBundleReference.Path);
            _assetFromBundle = new AssetBundleAssetFile<TestScriptableAsset>(assetIdent, _assetBundle, _coroutineHost);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Object.DestroyImmediate(_coroutineHost.gameObject);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeLoaded()
        {
            // Arrange.
            var bundle = _assetBundle;
            var bundleAsset = _assetFromBundle;

            // Act.
            var loadedAsset = bundleAsset.Load();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(bundleAsset.IsLoaded);

            // Debug.
            Debug.Log($"Loaded [{loadedAsset.name}] from bundle {bundle.name}.");

            // Cleanup.
            yield return Free(bundle);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeUnloaded()
        {
            // Arrange.
            var bundle = _assetBundle;
            var bundleAsset =  _assetFromBundle;
            bundleAsset.Load();
            yield return null;

            // Act.
            bundleAsset.Unload();
            yield return null;

            // Assert.
            Assert.That(bundleAsset.IsLoaded, Is.False);

            // Debug.
            Debug.Log($"Loaded and unloaded [{bundleAsset.Name}] from bundle [{bundle.name}].");

            // Cleanup.
            yield return Free(bundle);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeLoadedAsync()
        {
            // Arrange.
            var bundle = _assetBundle;
            TestScriptableAsset loadedAsset = null;
            var assetFromBundle = _assetFromBundle;

            // Act.
            _stopwatch.Restart();
            assetFromBundle.LoadAsync(
                onLoaded: asset => loadedAsset = asset,
                onFailed: exception => throw exception);

            while (loadedAsset == null)
            {
                yield return null;
            }

            _stopwatch.Stop();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);

            // Debug.
            Debug.Log($"Loaded [{loadedAsset.name}] " +
                      $"from bundle [{assetFromBundle.Name}] " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

            // Cleanup.
            yield return Free(bundle);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeLoadedAsyncWithProgress()
        {
            // Arrange.
            var callbacksCalled = false;
            var callbackCalls = 0;
            var bundle = _assetBundle;
            var assetFromBundle = _assetFromBundle;

            // Act.
            _stopwatch.Restart();
            assetFromBundle.LoadAsync(onLoaded: null,
                onProgress: _ =>
                {
                    callbackCalls++;
                    callbacksCalled = true;
                },
                onFailed: exception => throw exception);

            while (assetFromBundle.IsLoaded == false)
            {
                yield return null;
            }

            _stopwatch.Stop();

            // Assert.
            Assert.That(callbacksCalled);
            Assert.That(callbackCalls, Is.GreaterThan(0));

            // Debug.
            Debug.Log("Progress event was called " +
                      $"{callbackCalls} times " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

            // Cleanup.
            yield return Free(bundle);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleSizeShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var bundle = _assetBundle;
            var bundleAsset = _assetFromBundle;
            bundleAsset.Load();

            // Act.
            var assetSize = bundleAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Debug.Log($"Size of [{bundleAsset.Name}] is {assetSize.ToHumanReadableString()}.");

            // Cleanup.
            yield return Free(bundle);
        }
    }
}