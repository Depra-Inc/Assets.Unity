// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
using Depra.Assets.Common;
using Depra.Assets.Extensions;
using Depra.Assets.Files.Resource;
using Depra.Assets.Tests.EditMode.Stubs;
using Depra.Assets.Tests.EditMode.Utils;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Depra.Assets.Tests.EditMode.References
{
	internal sealed class ResourcesReferenceTests
	{
		private const string ASSET_NAME = nameof(EditModeTestScriptableAsset);

		private string _projectPath;
		private DirectoryInfo _resourcesDirectory;
		private EditModeTestScriptableAsset _testAsset;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_projectPath = Path.Combine(
				UnityProject.ASSETS_FOLDER_NAME,
				UnityProject.RESOURCES_FOLDER_NAME,
				ASSET_NAME + AssetTypes.BASE).ToUnixPath();

			(_resourcesDirectory = new DirectoryInfo(Path.GetDirectoryName(_projectPath)!)).CreateIfNotExists();
			_testAsset = TestEnvironment.CreateAsset<EditModeTestScriptableAsset>(_projectPath);
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (TestEnvironment.TryDeleteAsset(_testAsset))
			{
				_resourcesDirectory.DeleteIfEmpty();
			}
		}

		[Test]
		public void IsNull_TrueWhenProjectPathIsEmpty()
		{
			// Arrange:
			var reference = new ResourcesReference { _projectPath = string.Empty };

			// Act & Assert:
			Assert.IsTrue(reference.IsNull);
		}

		[Test]
		public void IsNull_FalseWhenProjectPathIsNotEmpty()
		{
			// Arrange:
			var reference = new ResourcesReference { _projectPath = _projectPath };

			// Act & Assert:
			Assert.IsFalse(reference.IsNull);
		}

		[Test]
		public void ResourcesPath_ReturnsRelativePath()
		{
			// Arrange:
			// ReSharper disable InlineTemporaryVariable
			const string EXPECTED_PATH = ASSET_NAME;
			// ReSharper restore InlineTemporaryVariable
			var reference = new ResourcesReference
			{
				_projectPath = _projectPath,
				_objectAsset = AssetDatabase.LoadAssetAtPath<Object>(_projectPath)
			};

			// Act:
			var actualPath = reference.Path.Relative;

			// Assert:
			Assert.AreEqual(EXPECTED_PATH, actualPath);
		}

		[Test]
		public void ProjectPath_ReturnsAssetPathInEditor()
		{
			// Arrange:
			var expectedPath = _projectPath;
			var reference = new ResourcesReference
			{
				_objectAsset = AssetDatabase.LoadAssetAtPath<Object>(_projectPath)
			};

			// Act:
			var actualPath = reference.Path.Project;

			// Act & Assert.
			Assert.AreEqual(expectedPath, actualPath);
		}
	}
}