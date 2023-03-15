using System.IO;
using Depra.Assets.Runtime.Files.Database;
using Depra.Assets.Tests.Common;
using NUnit.Framework;
using UnityEditor;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Tests.EditMode
{
    [TestFixture(TestOf = typeof(DatabaseAsset<>))]
    internal sealed class CreatingAssetsUsingAssetDatabase
    {
        private const string ASSET_NAME = "TestAsset";
        private const string ASSET_EXTENSION = AssetTypes.BASE;

        private string _assetPath;

        [SetUp]
        public void Setup() =>
            _assetPath = Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME);

        [TearDown]
        public void TearDown()
        {
            var absolutePath = Path.Combine(_assetPath, ASSET_NAME + ASSET_EXTENSION);
            AssetDatabase.DeleteAsset(absolutePath);
        }

        [Test]
        public void ScriptableAssetShouldBeCreated()
        {
            // Arrange.
            var assetFactory = new DatabaseAsset<TestAsset>(_assetPath, ASSET_NAME, typeExtension: ASSET_EXTENSION);

            // Act.
            var loadedAsset = assetFactory.Load();

            // Assert.
            Assert.IsNotNull(loadedAsset);
            Assert.IsTrue(assetFactory.IsLoaded);
        }

        [Test]
        public void ScriptableAssetShouldBeDeleted()
        {
            // Arrange.
            var assetFactory = new DatabaseAsset<TestAsset>(_assetPath, ASSET_NAME, typeExtension: ASSET_EXTENSION);
            var createdAsset = assetFactory.Load();

            // Act.
            assetFactory.Unload();

            // Assert.
            Assert.IsNotNull(createdAsset);
            Assert.IsFalse(assetFactory.IsLoaded);
        }
    }
}