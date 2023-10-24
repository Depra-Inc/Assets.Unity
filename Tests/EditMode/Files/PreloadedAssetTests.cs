// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Threading;
using Depra.Assets.Editor.Files;
using Depra.Assets.Files;
using Depra.Assets.Extensions;
using Depra.Assets.Tests.EditMode.Stubs;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Depra.Assets.Tests.EditMode.Files
{
	internal sealed class PreloadedAssetTests
	{
		private const int CANCEL_DELAY = 1000;

		private Object _testInstance;
		private Object[] _initialPreloadedAssets;
		private IAssetFile<EditModeTestScriptableAsset> _childAsset;

		[OneTimeSetUp]
		public void OneTimeSetup() =>
			_childAsset = new FakeAssetFile(
				new FakeAssetUri(nameof(EditModeTestScriptableAsset)));

		[SetUp]
		public void Setup()
		{
			_initialPreloadedAssets = PlayerSettings.GetPreloadedAssets();
			_testInstance = ScriptableObject.CreateInstance<EditModeTestScriptableAsset>();
			PlayerSettings.SetPreloadedAssets(new[] { _testInstance });
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(_testInstance);
			PlayerSettings.SetPreloadedAssets(_initialPreloadedAssets);
		}

		[Test]
		public void Load_ShouldSucceed()
		{
			// Arrange:
			var preloadedAsset = new PreloadedAsset<EditModeTestScriptableAsset>(_childAsset);

			// Act:
			var loadedAsset = preloadedAsset.Load();

			// Assert:
			Assert.That(loadedAsset, Is.Not.Null);
			Assert.That(preloadedAsset.IsLoaded);

			// Debug:
			TestContext.WriteLine($"{nameof(EditModeTestScriptableAsset)} loaded from {nameof(PlayerSettings)}.");
		}

		[Test]
		public void LoadMultiple_ShouldSucceed()
		{
			// Arrange:
			var resourceAsset = new PreloadedAsset<EditModeTestScriptableAsset>(_childAsset);

			// Act:
			var firstLoadedAsset = resourceAsset.Load();
			var secondLoadedAsset = resourceAsset.Load();

			// Assert:
			Assert.That(firstLoadedAsset, Is.Not.Null);
			Assert.That(secondLoadedAsset, Is.Not.Null);
			Assert.That(firstLoadedAsset, Is.EqualTo(secondLoadedAsset));

			// Debug:
			TestContext.WriteLine($"{firstLoadedAsset.name} loaded from {nameof(PlayerSettings)}.");
		}

		[Test]
		public void LoadAsync_ShouldSucceed() => ATask.Void(async () =>
		{
			// Arrange:
			var preloadedAsset = new PreloadedAsset<EditModeTestScriptableAsset>(_childAsset);
			var cancellationToken = new CancellationTokenSource(CANCEL_DELAY).Token;

			// Act:
			var loadedAsset = await preloadedAsset.LoadAsync(cancellationToken: cancellationToken);

			// Assert:
			Assert.That(loadedAsset, Is.Not.Null);
			Assert.That(preloadedAsset.IsLoaded);

			// Debug:
			TestContext.WriteLine($"{loadedAsset.name} loaded from {nameof(PlayerSettings)}.");
		});

		[Test]
		public void Unload_ShouldSucceed()
		{
			// Arrange:
			var preloadedAsset = new PreloadedAsset<EditModeTestScriptableAsset>(_childAsset);
			preloadedAsset.Load();

			// Act:
			preloadedAsset.Unload();

			// Assert:
			Assert.That(preloadedAsset.IsLoaded, Is.False);

			// Debug:
			TestContext.WriteLine($"{preloadedAsset.Metadata.Uri.Relative} unloaded from {nameof(PlayerSettings)}.");
		}

		[Test]
		public void SizeOfLoadedAsset_ShouldNotBeZeroOrUnknown()
		{
			// Arrange:
			var preloadedAsset = new PreloadedAsset<EditModeTestScriptableAsset>(_childAsset);
			preloadedAsset.Load();

			// Act:
			var assetSize = preloadedAsset.Metadata.Size;

			// Assert:
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

			// Debug:
			TestContext.WriteLine($"Size of {preloadedAsset.Metadata.Uri.Relative} is {assetSize.ToString()}.");
		}
	}
}