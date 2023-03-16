using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Tests.Common.Types;
using Depra.Assets.Tests.PlayMode.Exceptions;
using Depra.Assets.Tests.PlayMode.Utils;
using Depra.Coroutines.Unity.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode
{
    [TestFixture(TestOf = typeof(AssetBundleAssetFile<>))]
    internal sealed class LoadingAssetsFromBundles
    {
        private static RuntimeCoroutineHost _coroutineHost;
        private Stopwatch _stopwatch;
        private AssetIdent _assetIdent;

        private static IEnumerable<AssetBundleFile> AllBundles() =>
            Load.AllBundles(_coroutineHost);

        private static IEnumerable<AssetIdent> AssetIdents()
        {
            var assetBundle = Load.AssetBundle();
            var assetNames = assetBundle.Assets;
            if (assetNames == null || assetNames.Length == 0)
            {
                throw new TestAssetsConfigurationException(assetBundle.BundleName);
            }

            foreach (var assetName in assetNames)
            {
                yield return new AssetIdent(assetName, assetBundle.Path);
            }
        }

        private static IEnumerator Free(AssetBundleFile assetBundleFile)
        {
            assetBundleFile.Unload();
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
        public IEnumerator AssetFromBundleShouldBeLoaded(
            [ValueSource(nameof(AssetIdents))] AssetIdent assetIdent,
            [ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.
            var bundleAsset = new AssetBundleAssetFile<TestAsset>(assetIdent, assetBundleFile);

            // Act.
            var loadedAsset = bundleAsset.Load();

            // Assert.
            Assert.IsNotNull(loadedAsset);
            Assert.IsTrue(bundleAsset.IsLoaded);

            // Debug.
            Debug.Log($"Loaded [{loadedAsset.name}] from bundle {assetBundleFile.Name}.");

            yield return Free(assetBundleFile);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeUnloaded(
            [ValueSource(nameof(AssetIdents))] AssetIdent assetIdent,
            [ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.
            var bundleAsset = new AssetBundleAssetFile<TestAsset>(assetIdent, assetBundleFile);
            bundleAsset.Load();
            yield return null;

            // Act.
            bundleAsset.Unload();
            yield return null;

            // Assert.
            Assert.IsFalse(bundleAsset.IsLoaded);

            // Debug.
            Debug.Log($"Loaded and unloaded [{bundleAsset.Name}] from bundle [{assetBundleFile.Name}].");

            yield return Free(assetBundleFile);
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeLoadedAsync(
            [ValueSource(nameof(AssetIdents))] AssetIdent assetIdent,
            [ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
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

            // Assert.
            Assert.NotNull(loadedAsset);

            // Debug.
            Debug.Log($"Loaded [{loadedAsset.name}] " +
                      $"from bundle [{assetBundleFile.Name}] " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

            yield return Free(assetBundleFile);
        }
    }
}