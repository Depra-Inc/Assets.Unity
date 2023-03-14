using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Bundle.Files;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Tests.Common;
using Depra.Assets.Tests.PlayMode.Utils;
using NUnit.Framework;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode
{
    [TestFixture(TestOf = typeof(AssetBundleAssetFile<>))]
    internal sealed class AssetBundleAssetFileTests
    {
        private Stopwatch _stopwatch;
        private AssetIdent _assetIdent;

        [SetUp]
        public void Setup()
        {
            _stopwatch = new Stopwatch();
        }

        [Test]
        public void LoadAssetFromBundle_WhenFileLoadedSuccessfully_ExpectAssetNotNull(
            [ValueSource(nameof(AssetIdents))] AssetIdent assetIdent,
            [ValueSource(nameof(AssetBundles))] AssetBundleFile assetBundleFile)
        {
            // Arrange.
            using var assetBundleAssetFile = new AssetBundleAssetFile<TestAsset>(_assetIdent, assetBundleFile);

            // Act.
            var loadedAsset = assetBundleAssetFile.Load();

            // Assert.
            Assert.IsNotNull(loadedAsset);
        }

        [UnityTest]
        public IEnumerator LoadAssetBundleAsync_WhenFileLoadedSuccessfully_ExpectAssetBundleNotNull(
            [ValueSource(nameof(AssetIdents))] AssetIdent assetIdent,
            [ValueSource(nameof(AssetBundles))] AssetBundleFile assetBundleFile)
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
            Debug.Log($"Duration of loading {nameof(TestAsset)} " +
                      $"from {assetBundleFile.GetType().Name}" +
                      $" (in ticks) = {_stopwatch.ElapsedTicks}");

            // Assert.
            Assert.NotNull(loadedAsset);

            assetBundleFile.Unload();
        }

        public static IEnumerable<AssetIdent> AssetIdents() =>
            Load.AssetIdents(nameof(AssetBundleAssetFileTests));

        private static IEnumerable<AssetBundleFile> AssetBundles() =>
            Load.AllBundles(nameof(AssetBundleAssetFileTests));
    }
}