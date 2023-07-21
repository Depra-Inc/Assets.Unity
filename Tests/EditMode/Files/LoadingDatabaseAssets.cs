// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using Depra.Assets.Unity.Runtime.Common;
using Depra.Assets.Unity.Runtime.Files.Database;
using Depra.Assets.Unity.Tests.EditMode.Stubs;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using static Depra.Assets.Unity.Runtime.Common.Constants;
using static Depra.Assets.Unity.Runtime.Common.Paths;
using static Tests.EditMode.StaticData;

namespace Depra.Assets.Unity.Tests.EditMode.Files
{
    [TestFixture(TestOf = typeof(DatabaseAsset<>))]
    internal sealed class LoadingDatabaseAssets
    {
        private const string ASSET_TYPE_EXTENSION = AssetTypes.BASE;
        private const string ASSET_NAME = nameof(EditModeTestScriptableAsset);

        private DatabaseAssetIdent _existingAssetIdent;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var relativeDirectory = PACKAGES_FOLDER_NAME + "/" + 
                                    FullModuleName + "/" + 
                                    TESTS_FOLDER_NAME + "/" +
                                    nameof(EditMode) + "/" + 
                                    ASSETS_FOLDER_NAME;
            var temp = Path.Combine(PACKAGES_FOLDER_NAME, FullModuleName, TESTS_FOLDER_NAME,
                nameof(EditMode), ASSETS_FOLDER_NAME);
            
            _existingAssetIdent = new DatabaseAssetIdent(
                relativeDirectory: relativeDirectory,
                name: ASSET_NAME,
                extension: ASSET_TYPE_EXTENSION);
        }

        [Test]
        public void Load_ShouldSucceed()
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<EditModeTestScriptableAsset>(_existingAssetIdent);

            // Act.
            var loadedAsset = databaseAsset.Load();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(databaseAsset.IsLoaded);

            // Debug.
            TestContext.WriteLine($"Created {loadedAsset.name} at path: {_existingAssetIdent.AbsolutePath}.");
        }

        [Test]
        public void Unload_ShouldSucceed()
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<EditModeTestScriptableAsset>(_existingAssetIdent);
            var createdAsset = databaseAsset.Load();

            // Act.
            databaseAsset.Unload();

            // Assert.
            Assert.That(createdAsset, Is.Not.Null);
            Assert.That(databaseAsset.IsLoaded, Is.False);

            // Debug.
            TestContext.WriteLine(
                $"Deleted {nameof(EditModeTestScriptableAsset)} at path: {_existingAssetIdent.AbsolutePath}.");
        }

        [Test]
        public void SizeOfLoadedAsset_ShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<EditModeTestScriptableAsset>(_existingAssetIdent);
            databaseAsset.Load();

            // Act.
            var assetSize = databaseAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            TestContext.WriteLine($"Size of {databaseAsset.Ident.RelativeUri} is {assetSize.ToHumanReadableString()}.");
        }
    }
}