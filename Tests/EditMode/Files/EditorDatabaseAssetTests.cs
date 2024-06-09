// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
using System.Linq;
using Depra.Assets.Common;
using Depra.Assets.Editor.Files;
using Depra.Assets.Extensions;
using Depra.Assets.Files.Database;
using Depra.Assets.Tests.EditMode.Stubs;
using Depra.Assets.Tests.EditMode.Utils;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using static Depra.Assets.Common.UnityProject;

namespace Depra.Assets.Tests.EditMode.Files
{
	internal sealed class EditorDatabaseAssetTests
	{
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
				name: nameof(NonExistentScriptableAsset),
				extension: AssetTypes.BASE);

			_existentAssetUri = new DatabaseAssetUri(
				relativeDirectory: projectResourcesPath,
				name: nameof(EditModeTestScriptableAsset),
				extension: AssetTypes.BASE);

			_existentAssetUri.Directory.Require();
			_existentAsset = TestEnvironment.CreateAsset<EditModeTestScriptableAsset>(_existentAssetUri.Relative);
		}

		[TearDown]
		public void TearDown()
		{
			if (TestEnvironment.TryDeleteAsset(_nonExistentAsset))
			{
				TestEnvironment.CleanupDirectory(_nonExistentAssetUri.Directory);
			}
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (TestEnvironment.TryDeleteAsset(_existentAsset))
			{
				TestEnvironment.CleanupDirectory(_existentAssetUri.Directory);
			}
		}

		[Test]
		public void Load_ExistentAsset_ShouldSucceed()
		{
			// Arrange:
			var databaseAsset = new EditorDatabaseAsset<EditModeTestScriptableAsset>(_existentAssetUri);

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
			var databaseAsset = new EditorDatabaseAsset<NonExistentScriptableAsset>(_nonExistentAssetUri);

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
			var databaseAsset = new EditorDatabaseAsset<EditModeTestScriptableAsset>(_existentAssetUri);
			var createdAsset = databaseAsset.Load();

			// Act:
			databaseAsset.Unload();

			// Assert:
			Assert.That(createdAsset, Is.Not.Null);
			Assert.That(databaseAsset.IsLoaded, Is.False);

			// Debug:
			TestContext.WriteLine(
				$"Deleted {nameof(EditModeTestScriptableAsset)} at path: {_existentAssetUri.Absolute}.");
		}

		[Test]
		public void Size_OfLoadedAsset_ShouldNotBeZeroOrUnknown()
		{
			// Arrange:
			var databaseAsset = new EditorDatabaseAsset<EditModeTestScriptableAsset>(_existentAssetUri);
			databaseAsset.Load();

			// Act:
			var assetSize = databaseAsset.Metadata.Size;

			// Assert:
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

			// Debug:
			TestContext.WriteLine($"Size of {databaseAsset.Metadata.Uri.Relative} is {assetSize.ToString()}.");
		}

		[Test]
		public void Dependencies_OfAsset_ShouldContainsScript()
		{
			// Arrange:
			var databaseAsset = new EditorDatabaseAsset<EditModeTestScriptableAsset>(_existentAssetUri);

			// Act:
			var dependencies = databaseAsset.Dependencies().ToArray();

			// Assert:
			Assert.That(dependencies.Length, Is.EqualTo(1));
			Assert.That(dependencies.FirstOrDefault(x => x.Relative.Contains(nameof(EditModeTestScriptableAsset))),
				Is.Not.Null);

			// Debug:
			TestContext.WriteLine($"Dependencies of {databaseAsset.Metadata.Uri.Relative} is {dependencies.Flatten()}.");
		}
	}
}