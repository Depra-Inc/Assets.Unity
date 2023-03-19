using System.Diagnostics.CodeAnalysis;
using System.IO;
using Depra.Assets.Runtime.Files.Database;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Tests.EditMode.Files
{
    [TestFixture(TestOf = typeof(DatabaseAsset<>))]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    internal sealed class CreatingAssetsUsingAssetDatabase
    {
        private const string ASSET_NAME = "TestAsset";
        private const string ASSET_TYPE_EXTENSION = AssetTypes.BASE;

        private string _directoryName;
        private string _absoluteAssetPath;

        [SetUp]
        public void Setup()
        {
            _directoryName = RESOURCES_FOLDER_NAME;
            _absoluteAssetPath = Path.Combine(ASSETS_FOLDER_NAME, _directoryName, ASSET_NAME + ASSET_TYPE_EXTENSION);
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(_absoluteAssetPath);
        }

        [Test]
        public void ScriptableAssetShouldBeCreated()
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<ScriptableAsset>(_directoryName, ASSET_NAME, ASSET_TYPE_EXTENSION);

            // Act.
            var loadedAsset = databaseAsset.Load();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(databaseAsset.IsLoaded);

            // Debug.
            Debug.Log($"Created [{loadedAsset.name}] with path: {databaseAsset.Path}.");
        }

        [Test]
        public void ScriptableAssetShouldBeDeleted()
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<ScriptableAsset>(_directoryName, ASSET_NAME, ASSET_TYPE_EXTENSION);
            var createdAsset = databaseAsset.Load();

            // Act.
            databaseAsset.Unload();

            // Assert.
            Assert.That(createdAsset, Is.Not.Null);
            Assert.That(databaseAsset.IsLoaded, Is.False);

            // Debug.
            Debug.Log($"Created and deleted [{nameof(ScriptableAsset)}] with path: {databaseAsset.Path}.");
        }

        private sealed class ScriptableAsset : ScriptableObject { }
    }
}