// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Depra.Assets.Runtime.Files.Database;
using Depra.Assets.Runtime.Files.Structs;
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
            _directoryName = ResourcesFolderName;
            _absoluteAssetPath = Path.Combine(AssetsFolderName, _directoryName, ASSET_NAME + ASSET_TYPE_EXTENSION);
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(_absoluteAssetPath);
        }

        [Test]
        public void AssetShouldBeCreated()
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<ScriptableAsset>(_directoryName, ASSET_NAME, ASSET_TYPE_EXTENSION);

            // Act.
            var loadedAsset = databaseAsset.Load();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(databaseAsset.IsLoaded);

            // Debug.
            Debug.Log($"Created [{loadedAsset.name}] at path: {databaseAsset.Path}.");
        }

        [Test]
        public void LoadedAssetShouldBeDeleted()
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
            Debug.Log($"Created and deleted [{nameof(ScriptableAsset)}] " +
                      $"at path: {databaseAsset.Path}.");
        }
        
        [Test]
        public void AssetSizeShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<ScriptableAsset>(_directoryName, ASSET_NAME, ASSET_TYPE_EXTENSION);
            databaseAsset.Load();
            
            // Act.
            var assetSize = databaseAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Debug.Log($"Size of [{databaseAsset.Name}] is {assetSize.ToHumanReadableString()}.");
        }

        private sealed class ScriptableAsset : ScriptableObject { }
    }
}