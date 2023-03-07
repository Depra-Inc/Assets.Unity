using System;
using Depra.Assets.Runtime.Bundle.Files;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Tests.Common;
using NUnit.Framework;
using static Depra.Assets.Tests.EditMode.Bundle.AssetBundlesTestUtils;

namespace Depra.Assets.Tests.EditMode.Bundle
{
    [TestFixture(TestOf = typeof(AssetBundleAssetFile))]
    internal sealed class AssetBundleAssetFileTests
    {
        private const string ASSET_NAME = "TestAsset";
        private static readonly Type ASSET_TYPE = typeof(TestAsset);

        private TypedAssetIdent _assetIdent;

        [SetUp]
        public void Setup()
        {
            _assetIdent = new TypedAssetIdent(ASSET_TYPE, new AssetIdent(ASSET_NAME));
        }

        [Test]
        public void WhenLoadingAFileFromBundle_ThenTheFileWasLoadedSuccessfully(
            [ValueSource(nameof(AssetBundleLoaders))]
            AssetBundleFile assetBundleFile)
        {
            // Arrange
            using var assetBundleAssetFile = new AssetBundleAssetFile(_assetIdent, assetBundleFile);

            // Act.
            var loadedAsset = assetBundleAssetFile.Load();
            PrintAllAssetNames(assetBundleFile);

            // Assert.
            Assert.IsNotNull(loadedAsset);
        }
    }
}