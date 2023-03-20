using System.Collections;
using System.Diagnostics;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files;
using Depra.Assets.Runtime.Files.Bundles.Files;
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
        private AssetIdent _assetIdent;
        private AssetBundle _testAssetBundle;
        private TestCoroutineHost _coroutineHost;

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
            _testAssetBundle = AssetBundle.LoadFromFile(assetBundleReference.AbsolutePath);
            _assetIdent = new AssetIdent(assetBundleReference.SingleAssetName, assetBundleReference.Path);
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
            var ident = _assetIdent;
            var bundle = _testAssetBundle;
            var bundleAsset = new AssetBundleAssetFile<TestScriptableAsset>(ident, bundle, _coroutineHost);

            // Act.
            var loadedAsset = bundleAsset.Load();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(bundleAsset.IsLoaded);

            // Debug.
            Debug.Log($"Loaded [{loadedAsset.name}] from bundle {bundle.name}.");

            yield return Free(bundle);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeUnloaded()
        {
            // Arrange.
            var ident = _assetIdent;
            var bundle = _testAssetBundle;
            var bundleAsset = new AssetBundleAssetFile<TestScriptableAsset>(ident, bundle, _coroutineHost);
            bundleAsset.Load();
            yield return null;

            // Act.
            bundleAsset.Unload();
            yield return null;

            // Assert.
            Assert.That(bundleAsset.IsLoaded, Is.False);

            // Debug.
            Debug.Log($"Loaded and unloaded [{bundleAsset.Name}] from bundle [{bundle.name}].");

            yield return Free(bundle);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeLoadedAsync()
        {
            // Arrange.
            var ident = _assetIdent;
            var bundle = _testAssetBundle;
            TestScriptableAsset loadedAsset = null;
            var assetFromBundle = new AssetBundleAssetFile<TestScriptableAsset>(ident, bundle, _coroutineHost);

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

            yield return Free(bundle);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeLoadedAsyncWithProgress()
        {
            // Arrange.
            var callbacksCalled = false;
            var callbackCalls = 0;
            var ident = _assetIdent;
            var bundle = _testAssetBundle;
            var assetFromBundle = new AssetBundleAssetFile<TestScriptableAsset>(ident, bundle, _coroutineHost);

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

            // Debug.
            Debug.Log("Progress event was called " +
                      $"{callbackCalls} times " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

            yield return Free(bundle);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleSizeShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var ident = _assetIdent;
            var bundle = _testAssetBundle;
            var bundleAsset = new AssetBundleAssetFile<TestScriptableAsset>(ident, bundle, _coroutineHost);
            bundleAsset.Load();

            // Act.
            var assetSize = bundleAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Debug.Log($"Size of [{ident.Name}] is {assetSize.ToHumanReadableString()}.");

            yield return Free(bundle);
        }
    }
}