// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Extensions;
using Depra.Assets.Runtime.Files.Database;
using Depra.Assets.Tests.EditMode.Stubs;
using Depra.Assets.Tests.EditMode.Utils;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using static Depra.Assets.Runtime.Common.Paths;

namespace Depra.Assets.Tests.EditMode.Files
{
	[TestFixture(TestOf = typeof(DatabaseAsset<>))]
	internal sealed class DatabaseAssetTests
	{
		private const string ASSET_TYPE_EXTENSION = AssetTypes.BASE;
		private const string EXISTENT_ASSET_NAME = nameof(EditModeTestScriptableAsset);
		private const string NON_EXISTENT_ASSET_NAME = nameof(NonExistentScriptableAsset);

		private DatabaseAssetIdent _existentAssetIdent;
		private EditModeTestScriptableAsset _existentAsset;

		private DatabaseAssetIdent _nonExistentAssetIdent;
		private NonExistentScriptableAsset _nonExistentAsset;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			var projectResourcesPath = Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME);

			_nonExistentAssetIdent = new DatabaseAssetIdent(
				relativeDirectory: projectResourcesPath,
				name: NON_EXISTENT_ASSET_NAME,
				extension: ASSET_TYPE_EXTENSION);

			_existentAssetIdent = new DatabaseAssetIdent(
				relativeDirectory: projectResourcesPath,
				name: EXISTENT_ASSET_NAME,
				extension: ASSET_TYPE_EXTENSION);

			_existentAssetIdent.Directory.CreateIfNotExists();
			_existentAsset = TestEnvironment.CreateAsset<EditModeTestScriptableAsset>(_existentAssetIdent.RelativePath);
		}

		[TearDown]
		public void TearDown()
		{
			if (TestEnvironment.TryDeleteAsset(_nonExistentAsset))
			{
				_nonExistentAssetIdent.Directory.DeleteIfEmpty();
			}
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (TestEnvironment.TryDeleteAsset(_existentAsset))
			{
				_existentAssetIdent.Directory.DeleteIfEmpty();
			}
		}

		[Test]
		public void Load_ExistentAsset_ShouldSucceed()
		{
			// Arrange.
			var databaseAsset = new DatabaseAsset<EditModeTestScriptableAsset>(_existentAssetIdent);

			// Act.
			var loadedAsset = databaseAsset.Load();

			// Assert.
			Assert.That(loadedAsset, Is.Not.Null);
			Assert.That(databaseAsset.IsLoaded);

			// Debug.
			TestContext.WriteLine($"Created {loadedAsset.name} at path: {_existentAssetIdent.AbsolutePath}.");
		}

		[Test]
		public void Load_NonExistentAsset_ShouldSucceed()
		{
			// Arrange.
			var databaseAsset = new DatabaseAsset<NonExistentScriptableAsset>(_nonExistentAssetIdent);

			// Act.
			_nonExistentAsset = databaseAsset.Load();

			// Assert.
			Assert.That(_nonExistentAsset, Is.Not.Null);
			Assert.That(databaseAsset.IsLoaded);

			// Debug.
			TestContext.WriteLine($"Created {_nonExistentAsset.name} at path: {_existentAssetIdent.AbsolutePath}.");
		}

		[Test]
		public void Unload_ShouldSucceed()
		{
			// Arrange.
			var databaseAsset = new DatabaseAsset<EditModeTestScriptableAsset>(_existentAssetIdent);
			var createdAsset = databaseAsset.Load();

			// Act.
			databaseAsset.Unload();

			// Assert.
			Assert.That(createdAsset, Is.Not.Null);
			Assert.That(databaseAsset.IsLoaded, Is.False);

			// Debug.
			TestContext.WriteLine($"Deleted {nameof(EditModeTestScriptableAsset)} at path: {_existentAssetIdent.AbsolutePath}.");
		}

		[Test]
		public void Size_OfLoadedAsset_ShouldNotBeZeroOrUnknown()
		{
			// Arrange.
			var databaseAsset = new DatabaseAsset<EditModeTestScriptableAsset>(_existentAssetIdent);
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