// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Bundles.Idents;
using Depra.Assets.Runtime.Files.Bundles.Sources;
using Depra.Assets.Tests.PlayMode.Stubs;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Depra.Assets.Tests.PlayMode.Files
{
	[TestFixture(TestOf = typeof(AssetBundleFile))]
	internal sealed class LocalAssetBundleFileTests
	{
		private const string TEST_BUNDLE_NAME = "test";

		private static IEnumerable<AssetBundleFile> AllBundles()
		{
			var assetBundlesDirectory = new TestAssetBundlesDirectory();
			var bundleIdent = new AssetBundleIdent(TEST_BUNDLE_NAME, assetBundlesDirectory.ProjectRelativePath);

			yield return new AssetBundleFile(bundleIdent, new AssetBundleFromFile());
			yield return new AssetBundleFile(bundleIdent, new AssetBundleFromMemory());
			yield return new AssetBundleFile(bundleIdent, new AssetBundleFromStream());
		}

		private static IEnumerable<AssetBundleFile> InvalidBundles()
		{
			var invalidIdent = AssetBundleIdent.Invalid;
			yield return new AssetBundleFile(invalidIdent, new AssetBundleFromFile());
			yield return new AssetBundleFile(invalidIdent, new AssetBundleFromMemory());
			yield return new AssetBundleFile(invalidIdent, new AssetBundleFromStream());
		}

		private Stopwatch _stopwatch;
		private AssetBundle _loadedBundle;

		[OneTimeSetUp]
		public void OneTimeSetup() =>
			_stopwatch = new Stopwatch();

		[TearDown]
		public void TearDown() =>
			AssetBundle.UnloadAllAssetBundles(true);

		[Test]
		public void Load_ShouldSucceed([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
		{
			// Arrange & Act.
			_loadedBundle = bundleFile.Load();

			// Assert.
			Assert.That(_loadedBundle, Is.Not.Null);
			Assert.That(bundleFile.IsLoaded);

			// Debug.
			TestContext.WriteLine($"The bundle was loaded by path: {bundleFile.Ident.Uri}.");
		}

		[Test]
		public void InvalidBundleShouldThrowExceptionOnLoad(
			[ValueSource(nameof(InvalidBundles))] AssetBundleFile invalidBundleFile)
		{
			// Arrange.

			// Act.
			void Act() => invalidBundleFile.Load();

			// Assert.
			Assert.That(Act, Throws.InstanceOf<Exception>());
		}

		[UnityTest]
		public IEnumerator LoadAsync_ShouldSucceed([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile) =>
			UniTask.ToCoroutine(async () =>
			{
				// Arrange.

				// Act.
				_stopwatch.Restart();
				_loadedBundle = await bundleFile.LoadAsync();
				_stopwatch.Stop();

				// Assert.
				Assert.That(bundleFile.IsLoaded);
				Assert.That(_loadedBundle, Is.Not.Null);
				Assert.IsInstanceOf<AssetBundle>(_loadedBundle);

				// Debug.
				TestContext.WriteLine($"Loaded bundle {_loadedBundle.name} " +
				                      $"by path: {bundleFile.Ident.Uri} " +
				                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

				await UniTask.Yield();
			});

		[UnityTest]
		public IEnumerator LoadAsync_WithProgress_ShouldSucceed(
			[ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile) =>
			UniTask.ToCoroutine(async () =>
			{
				// Arrange.
				var callbackCalls = 0;
				var callbacksCalled = false;
				DownloadProgress lastProgress = default;

				// Act.
				_stopwatch.Restart();
				_loadedBundle = await bundleFile.LoadAsync(
					onProgress: progress =>
					{
						callbackCalls++;
						callbacksCalled = true;
						lastProgress = progress;
					});
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

				await UniTask.Yield();
			});

		[UnityTest]
		public IEnumerator LoadAsync_CancelBeforeStart_ShouldThrowTaskCanceledException(
			[ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
		{
			// Prepare.
			// I do not understand why the bundle does not have time to unload in TearDown.
			AssetBundle.UnloadAllAssetBundles(true);
			yield return null;

			// Arrange.
			var cts = new CancellationTokenSource();

			// Act.
			cts.Cancel();
			var loadTask = bundleFile.LoadAsync(cancellationToken: cts.Token);
			async Task Act() => await loadTask;

			// Assert.
			Assert.ThrowsAsync<TaskCanceledException>(Act);
		}

		[UnityTest]
		public IEnumerator LoadAsync_CancelDuringExecution_ShouldThrowTaskCanceledException(
			[ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
		{
			// Prepare.
			// I do not understand why the bundle does not have time to unload in TearDown.
			AssetBundle.UnloadAllAssetBundles(true);
			yield return null;

			// Arrange.
			var cts = new CancellationTokenSource();

			// Act.
			cts.CancelAfterSlim(1);
			var loadTask = bundleFile.LoadAsync(cancellationToken: cts.Token);
			async Task Act() => await loadTask;

			yield return new WaitUntil(() => cts.Token.IsCancellationRequested);

			// Assert.
			Assert.ThrowsAsync<TaskCanceledException>(Act);
		}

		[UnityTest]
		public IEnumerator SizeOfLoadedAsset_ShouldNotBeZeroOrUnknown(
			[ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
		{
			// Arrange.
			_loadedBundle = bundleFile.Load();
			yield return null;

			// Act.
			var bundleSize = bundleFile.Size;

			// Assert.
			Assert.That(bundleSize, Is.Not.EqualTo(FileSize.Zero));
			Assert.That(bundleSize, Is.Not.EqualTo(FileSize.Unknown));

			// Debug.
			TestContext.WriteLine($"Size of {bundleFile.Ident.RelativeUri} is {bundleSize.ToHumanReadableString()}.");

			yield return null;
		}

		[UnityTest]
		public IEnumerator Unload_ShouldSucceed([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
		{
			// Arrange.
			bundleFile.Load();
			yield return null;

			// Act.
			bundleFile.Unload();
			yield return null;

			// Assert.
			Assert.That(bundleFile.IsLoaded, Is.False);

			// Debug.
			TestContext.WriteLine($"The bundle with name {bundleFile.Ident.RelativeUri} was unloaded.");
		}
	}
}