// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Depra.Assets.Idents;
using Depra.Assets.Runtime.Extensions;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Tests.PlayMode.Stubs;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Depra.Assets.Tests.PlayMode.Files
{
	[TestFixture(TestOf = typeof(AssetBundleAssetFile<>))]
	internal sealed class AssetBundleAssetFileTests
	{
		private const string TEST_BUNDLE_NAME = "test";
		private const string TEST_ASSET_NAME = nameof(PlayModeTestScriptableAsset);

		private Stopwatch _stopwatch;
		private AssetBundle _assetBundle;
		private AssetBundleAssetFile<PlayModeTestScriptableAsset> _assetFromBundle;

		[OneTimeSetUp]
		public void OneTimeSetup() => _stopwatch = new Stopwatch();

		[SetUp]
		public void Setup()
		{
			var assetBundlesDirectory = new TestAssetBundlesDirectory();
			var assetBundlePath = Path.Combine(assetBundlesDirectory.AbsolutePath, TEST_BUNDLE_NAME);
			_assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
			var assetIdent = new AssetName(TEST_ASSET_NAME);
			_assetFromBundle = new AssetBundleAssetFile<PlayModeTestScriptableAsset>(assetIdent, _assetBundle);
		}

		[TearDown]
		public void TearDown()
		{
			_assetFromBundle.Unload();
			_assetBundle.Unload(true);
		}

		[Test]
		public void Load_ShouldSucceed()
		{
			// Arrange & Act.
			var loadedAsset = _assetFromBundle.Load();

			// Assert.
			Assert.That(loadedAsset, Is.Not.Null);
			Assert.That(_assetFromBundle.IsLoaded);

			// Debug.
			TestContext.WriteLine($"{loadedAsset.name} loaded from bundle with name: {_assetBundle.name}.");
		}

		[UnityTest]
		public IEnumerator Unload_ShouldSucceed()
		{
			// Arrange.
			_assetFromBundle.Load();
			yield return null;

			// Act.
			_assetFromBundle.Unload();
			yield return null;

			// Assert.
			Assert.That(_assetFromBundle.IsLoaded, Is.False);

			// Debug.
			TestContext.WriteLine($"{_assetFromBundle.Ident.RelativeUri} unloaded from bundle: {_assetBundle.name}.");
		}

		[UnityTest]
		public IEnumerator LoadAsync_ShouldSucceed() => ATask.ToCoroutine(async () =>
		{
			// Arrange.
			var cancellationToken = new CancellationTokenSource(1000).Token;

			// Act.
			_stopwatch.Restart();
			var loadedAsset = await _assetFromBundle.LoadAsync(cancellationToken: cancellationToken);
			_stopwatch.Stop();

			// Assert.
			Assert.That(loadedAsset, Is.Not.Null);
			Assert.IsInstanceOf<PlayModeTestScriptableAsset>(loadedAsset);

			// Debug.
			TestContext.WriteLine($"{loadedAsset.name} loaded " +
			                      $"from bundle {_assetBundle.name} " +
			                      $"in {_stopwatch.ElapsedMilliseconds} ms.");
		});

		[UnityTest]
		public IEnumerator LoadAsync_WithProgress_ShouldSucceed() => ATask.ToCoroutine(async () =>
		{
			// Arrange.
			var callbackCalls = 0;
			var callbacksCalled = false;
			var assetFromBundle = _assetFromBundle;
			DownloadProgress lastProgress = default;

			// Act.
			_stopwatch.Restart();
			await assetFromBundle.LoadAsync(
				onProgress: progress =>
				{
					callbackCalls++;
					callbacksCalled = true;
					lastProgress = progress;
				},
				CancellationToken.None);
			_stopwatch.Stop();

			// Assert.
			Assert.That(callbacksCalled);
			Assert.That(callbackCalls, Is.GreaterThan(0));
			Assert.That(lastProgress, Is.EqualTo(DownloadProgress.Full));

			// Debug.
			TestContext.WriteLine("Progress event was called " +
			                      $"{callbackCalls} times " +
			                      $"in {_stopwatch.ElapsedMilliseconds} ms. " +
			                      $"Last value is {lastProgress.NormalizedValue}.");
		});

		[Test]
		public void Load_InvalidAssetFromBundle_ShouldThrowAssetBundleFileNotLoadedException()
		{
			// Arrange.
			var invalidIdent = AssetName.Invalid;
			var invalidAssetFromBundle = new AssetBundleAssetFile<TestMonoAsset>(invalidIdent, _assetBundle);

			// Act.
			void Act() => invalidAssetFromBundle.Load();

			// Assert.
			Assert.That(Act, Throws.TypeOf<AssetBundleFileNotLoaded>());
		}

		[Test]
		public void SizeOfLoadedAsset_ShouldNotBeZeroOrUnknown()
		{
			// Arrange.
			_assetFromBundle.Load();

			// Act.
			var assetSize = _assetFromBundle.Size;

			// Assert.
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

			// Debug.
			TestContext.WriteLine($"Size of {_assetFromBundle.Ident.RelativeUri} " +
			                      $"is {assetSize.ToHumanReadableString()}.");
		}
	}
}