// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
using Depra.Assets.Common;
using Depra.Assets.Extensions;
using Depra.Assets.Files.Database;
using Depra.Assets.Tests.EditMode.Stubs;
using Depra.Assets.Tests.EditMode.Utils;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using static Depra.Assets.Common.UnityProject;

namespace Depra.Assets.Tests.EditMode.Files
{
	internal sealed class DatabaseAssetTests
	{
		private const string ASSET_TYPE_EXTENSION = AssetTypes.BASE;
		private const string EXISTENT_ASSET_NAME = nameof(EditModeTestScriptableAsset);
		private const string NON_EXISTENT_ASSET_NAME = nameof(NonExistentScriptableAsset);

		private DatabaseAssetUri _existentAssetUri;
		private EditModeTestScriptableAsset _existentAsset;

		private DatabaseAssetUri _nonExistentAssetUri;
		private NonExistentScriptableAsset _nonExistentAsset;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			var projectResourcesPath = Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME);

			_nonExistentAssetUri = new DatabaseAssetUri(
				relativeDirectory: projectResourcesPath,
				name: NON_EXISTENT_ASSET_NAME,
				extension: ASSET_TYPE_EXTENSION);

			_existentAssetUri = new DatabaseAssetUri(
				relativeDirectory: projectResourcesPath,
				name: EXISTENT_ASSET_NAME,
				extension: ASSET_TYPE_EXTENSION);

			_existentAssetUri.Directory.CreateIfNotExists();
			_existentAsset = TestEnvironment.CreateAsset<EditModeTestScriptableAsset>(_existentAssetUri.Relative);
		}

		[TearDown]
		public void TearDown()
		{
			if (TestEnvironment.TryDeleteAsset(_nonExistentAsset))
			{
				_nonExistentAssetUri.Directory.DeleteIfEmpty();
			}
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (TestEnvironment.TryDeleteAsset(_existentAsset))
			{
				_existentAssetUri.Directory.DeleteIfEmpty();
			}
		}

		[Test]
		public void Load_ExistentAsset_ShouldSucceed()
		{
			// Arrange:
			var databaseAsset = new DatabaseAsset<EditModeTestScriptableAsset>(_existentAssetUri);

			// Act:
			var loadedAsset = databaseAsset.Load();

			// Assert:
			Assert.That(loadedAsset, Is.Not.Null);
			Assert.That(databaseAsset.IsLoaded);

			// Debug:
			TestContext.WriteLine($"Created {loadedAsset.name} at path: {_existentAssetUri.Absolute}.");
		}

		[Test]
		public void Load_NonExistentAsset_ShouldSucceed()
		{
			// Arrange:
			var databaseAsset = new DatabaseAsset<NonExistentScriptableAsset>(_nonExistentAssetUri);

			// Act:
			_nonExistentAsset = databaseAsset.Load();

			// Assert:
			Assert.That(_nonExistentAsset, Is.Not.Null);
			Assert.That(databaseAsset.IsLoaded);

			// Debug:
			TestContext.WriteLine($"Created {_nonExistentAsset.name} at path: {_existentAssetUri.Absolute}.");
		}

		[Test]
		public void Unload_ShouldSucceed()
		{
			// Arrange:
			var databaseAsset = new DatabaseAsset<EditModeTestScriptableAsset>(_existentAssetUri);
			var createdAsset = databaseAsset.Load();

			// Act:
			databaseAsset.Unload();

			// Assert:
			Assert.That(createdAsset, Is.Not.Null);
			Assert.That(databaseAsset.IsLoaded, Is.False);

			// Debug:
			TestContext.WriteLine($"Deleted {nameof(EditModeTestScriptableAsset)} at path: {_existentAssetUri.Absolute}.");
		}

		[Test]
		public void Size_OfLoadedAsset_ShouldNotBeZeroOrUnknown()
		{
			// Arrange:
			var databaseAsset = new DatabaseAsset<EditModeTestScriptableAsset>(_existentAssetUri);
			databaseAsset.Load();

			// Act:
			var assetSize = databaseAsset.Metadata.Size;

			// Assert:
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

			// Debug:
			TestContext.WriteLine($"Size of {databaseAsset.Metadata.Uri.Relative} is {assetSize.ToString()}.");
		}
	}
}