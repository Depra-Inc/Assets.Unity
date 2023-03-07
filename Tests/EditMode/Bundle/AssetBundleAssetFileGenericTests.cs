using Depra.Assets.Runtime.Bundle.Files;
using Depra.Assets.Tests.Common;
using NUnit.Framework;
using static Depra.Assets.Tests.EditMode.Bundle.AssetBundlesTestUtils;

namespace Depra.Assets.Tests.EditMode.Bundles
{
    [TestFixture(TestOf = typeof(AssetBundleAssetFile<>))]
    internal sealed class AssetBundleAssetFileGenericTests
    {
        private const string ASSET_NAME = "TestAsset";

        [Test]
        public void WhenLoadingAFileFromBundle_AndByGenericMethod_ThenTheFileWasLoadedSuccessfully(
            [ValueSource(nameof(AssetBundleLoaders))]
            AssetBundleFile assetBundleFile)
        {
            // Arrange.
            using var assetBundleAssetFile = new AssetBundleAssetFile<TestAsset>(assetBundleFile, ASSET_NAME);

            // Act.
            var loadedAsset = assetBundleAssetFile.Load();
            PrintAllAssetNames(assetBundleFile);

            // Assert.
            Assert.IsNotNull(loadedAsset);
        }
    }
}