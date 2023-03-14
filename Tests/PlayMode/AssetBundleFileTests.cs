using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Bundle.Files;
using Depra.Assets.Tests.PlayMode.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode
{
    [TestFixture(TestOf = typeof(AssetBundleFile))]
    internal sealed class AssetBundleFileTests
    {
        private Stopwatch _stopwatch;

        [SetUp]
        public void Setup() => _stopwatch = new Stopwatch();

        [Test]
        public void LoadAssetBundle_WhenFileLoadedSuccessfully_ExpectAssetBundleNotNull(
            [ValueSource(nameof(AllBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.

            // Act.
            var loadedAsset = assetBundleFile.Load();

            // Assert.
            Assert.IsNotNull(loadedAsset);

            assetBundleFile.Unload();
        }

        [UnityTest]
        public IEnumerator LoadAssetBundleAsync_WhenFileLoadedSuccessfully_ExpectAssetBundleNotNull(
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
            Debug.Log($"Duration of loading from {assetBundleFile.GetType().Name}" +
                      $" (in ticks) = {_stopwatch.ElapsedTicks}");

            // Assert.
            Assert.NotNull(loadedAssetBundle);

            assetBundleFile.Unload();
        }

        private static IEnumerable<AssetBundleFile> AllBundles() =>
            Load.AllBundles(nameof(AssetBundleFileTests));
    }
}