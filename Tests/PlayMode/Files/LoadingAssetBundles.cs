using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Bundles.IO;
using Depra.Assets.Runtime.Files.Bundles.Memory;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Tests.PlayMode.Types;
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

        private static IEnumerable<AssetBundleFile> AllBundles()
        {
            var assetBundleRef = TestAssetBundleRef.Load();
            var bundleIdent = new AssetIdent(assetBundleRef.BundleName, assetBundleRef.AbsoluteDirectoryPath);

            yield return new AssetBundleFromFile(bundleIdent, CoroutineHost);
            yield return new AssetBundleFromMemory(bundleIdent, CoroutineHost);
            yield return new AssetBundleFromStream(bundleIdent, CoroutineHost);
            //yield return new AssetBundleFromWeb(bundleIdent, CoroutineHost);
        }

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
        public IEnumerator BundleShouldBeLoaded([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.

            // Act.
            var loadedAssetBundle = bundleFile.Load();

            // Assert.
            Assert.That(loadedAssetBundle, Is.Not.Null);
            Assert.That(bundleFile.IsLoaded);

            // Debug.
            Debug.Log($"Loaded bundle [{loadedAssetBundle.name}] by path: [{bundleFile.Path}].");

            yield return Free(loadedAssetBundle);
        }

        [UnityTest]
        public IEnumerator BundleShouldBeUnloaded([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.
            bundleFile.Load();
            yield return null;

            // Act.
            bundleFile.Unload();
            yield return null;

            // Assert.
            Assert.That(bundleFile.IsLoaded, Is.False);

            // Debug.
            Debug.Log($"Loaded and unloaded bundle [{bundleFile.Name}] by path: {bundleFile.Path}.");
        }

        [UnityTest]
        public IEnumerator BundleShouldBeLoadedAsync([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.
            AssetBundle loadedBundle = null;

            // Act.
            _stopwatch.Restart();
            var asyncToken = bundleFile.LoadAsync(
                onLoaded: asset => loadedBundle = asset,
                onFailed: exception => throw exception);

            while (loadedBundle == null)
            {
                yield return null;
            }

            _stopwatch.Stop();

            // Assert.
            Assert.That(loadedBundle, Is.Not.Null);
            Assert.That(bundleFile.IsLoaded);
            Assert.That(asyncToken.IsCanceled, Is.False);

            // Debug.
            Debug.Log($"Loaded bundle [{loadedBundle.name}] " +
                      $"by path: [{bundleFile.Path}] " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

            yield return Free(loadedBundle);
        }

        [UnityTest]
        public IEnumerator BundleShouldBeLoadedWithProgress(
            [ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.
            var callbacksCalled = false;
            var callbackCalls = 0;
            AssetBundle loadedBundle = null;

            // Act.
            _stopwatch.Restart();
            bundleFile.LoadAsync(
                onLoaded: asset => loadedBundle = asset,
                onProgress: _ =>
                {
                    callbackCalls++;
                    callbacksCalled = true;
                },
                onFailed: exception => throw exception);

            while (bundleFile.IsLoaded == false)
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
            yield return Free(loadedBundle);
        }

        [UnityTest]
        public IEnumerator BundleLoadingShouldBeCanceled([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.
            AssetBundle loadedBundle = null;

            // Act.
            var asyncToken = bundleFile.LoadAsync(
                onLoaded: asset => loadedBundle = asset,
                onFailed: exception => throw exception);
            asyncToken.Cancel();

            // Assert.
            Assert.That(loadedBundle, Is.Null);
            Assert.That(asyncToken.IsCanceled);

            // Debug.
            Debug.Log($"Loading of bundle [{bundleFile.Name}] was canceled.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BundleSizeShouldNotBeZeroOrUnknown(
            [ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.

            // Act.
            var loadedBundle = bundleFile.Load();
            var bundleSize = bundleFile.Size;
            yield return null;

            // Assert.
            Assert.That(bundleSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(bundleSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Debug.Log($"Size of [{bundleFile.Name}] is {bundleSize.ToHumanReadableString()}.");

            yield return Free(loadedBundle);
        }
    }
}