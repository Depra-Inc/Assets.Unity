using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Bundles.IO;
using Depra.Assets.Runtime.Files.Bundles.Memory;
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
        public IEnumerator BundleShouldBeLoaded([ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.

            // Act.
            var loadedAssetBundle = assetBundleFile.Load();

            // Assert.
            Assert.That(loadedAssetBundle, Is.Not.Null);
            Assert.That(assetBundleFile.IsLoaded);

            // Debug.
            Debug.Log($"Loaded bundle [{loadedAssetBundle.name}] by path: [{assetBundleFile.Path}].");

            yield return Free(loadedAssetBundle);
        }

        [UnityTest]
        public IEnumerator BundleShouldBeUnloaded([ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.
            assetBundleFile.Load();
            yield return null;

            // Act.
            assetBundleFile.Unload();
            yield return null;

            // Assert.
            Assert.That(assetBundleFile.IsLoaded, Is.False);

            // Debug.
            Debug.Log($"Loaded and unloaded bundle [{assetBundleFile.Name}] by path: {assetBundleFile.Path}.");
        }

        [UnityTest]
        public IEnumerator BundleShouldBeLoadedAsync([ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.
            AssetBundle loadedAssetBundle = null;

            // Act.
            _stopwatch.Restart();
            assetBundleFile.LoadAsync(onLoaded: asset => loadedAssetBundle = asset,
                onFailed: exception => throw exception);

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
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

            yield return Free(loadedAssetBundle);
        }

        [UnityTest]
        public IEnumerator BundleSizeShouldNotBeZeroOrUnknown(
            [ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.

            // Act.
            var loadedAssetBundle = assetBundleFile.Load();
            var bundleSize = assetBundleFile.Size;
            yield return null;

            // Assert.
            Assert.That(bundleSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(bundleSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Debug.Log($"Size of [{assetBundleFile.Name}] is {bundleSize.ToHumanReadableString()}.");

            yield return Free(loadedAssetBundle);
        }
    }
}