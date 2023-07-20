// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;
using Depra.Assets.Unity.Runtime.Common;
using Depra.Assets.Unity.Runtime.Files.Database;
using Depra.Assets.Unity.Tests.EditMode.Stubs;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using UnityEditor;
using static Depra.Assets.Unity.Runtime.Common.Constants;
using static UnityEngine.Debug;
using Assert = NUnit.Framework.Assert;

namespace Depra.Assets.Unity.Tests.EditMode.Files
{
    [TestFixture(TestOf = typeof(DatabaseAsset<>))]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    internal sealed class LoadingDatabaseAssets
    {
        private const string ASSET_NAME = "TestAsset";
        private const string ASSET_TYPE_EXTENSION = AssetTypes.BASE;

        private DatabaseAssetIdent _assetIdent;

        [OneTimeSetUp]
        public void OneTimeSetup() => 
            _assetIdent = new DatabaseAssetIdent(RESOURCES_FOLDER_NAME, ASSET_NAME, ASSET_TYPE_EXTENSION);

        [TearDown]
        public void TearDown() => 
            AssetDatabase.DeleteAsset(_assetIdent.RelativePath);

        [Test]
        public void Load_ShouldSucceed()
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<FakeScriptableAsset>(_assetIdent);

            // Act.
            var loadedAsset = databaseAsset.Load();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(databaseAsset.IsLoaded);

            // Debug.
            Log($"Created {loadedAsset.name} at path: {_assetIdent.AbsolutePath}.");
        }

        [Test]
        public void Unload_ShouldSucceed()
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<FakeScriptableAsset>(_assetIdent);
            var createdAsset = databaseAsset.Load();

            // Act.
            databaseAsset.Unload();

            // Assert.
            Assert.That(createdAsset, Is.Not.Null);
            Assert.That(databaseAsset.IsLoaded, Is.False);

            // Debug.
            Log($"Deleted {nameof(FakeScriptableAsset)} at path: {_assetIdent.AbsolutePath}.");
        }

        [Test]
        public void SizeOfLoadedAsset_ShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<FakeScriptableAsset>(_assetIdent);
            databaseAsset.Load();

            // Act.
            var assetSize = databaseAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Log($"Size of {databaseAsset.Ident.RelativeUri} is {assetSize.ToHumanReadableString()}.");
        }
    }
}