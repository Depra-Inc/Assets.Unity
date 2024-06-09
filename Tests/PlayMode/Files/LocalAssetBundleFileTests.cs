// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Extensions;
using Depra.Assets.Files.Bundles;
using Depra.Assets.Files.Bundles.Sources;
using Depra.Assets.Tests.PlayMode.Stubs;
using Depra.Assets.Tests.PlayMode.Utils;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Depra.Assets.Tests.PlayMode.Files
{
	internal sealed class LocalAssetBundleFileTests
	{
		private const string TEST_BUNDLE_NAME = "test";

		private static IEnumerable<IAssetBundleSource> BundleSources()
		{
			yield return new AssetBundleFromFile();
			yield return new AssetBundleFromMemory();
			yield return new AssetBundleFromStream();
		}

		private Stopwatch _stopwatch;
		private AssetBundleUri _validUri;
		private AssetBundle _loadedBundle;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_stopwatch = new Stopwatch();
			_validUri = new AssetBundleUri(TEST_BUNDLE_NAME,
				new TestAssetBundlesDirectory().ProjectRelativePath);
		}

		[TearDown]
		public void TearDown() => AssetBundle.UnloadAllAssetBundles(true);

		[Test]
		public void Load_ShouldSucceed([ValueSource(nameof(BundleSources))] IAssetBundleSource source)
		{
			// Arrange:
			var bundleFile = new AssetBundleFile(_validUri, source);

			//Act:
			_loadedBundle = bundleFile.Load();

			// Assert:
			Assert.That(_loadedBundle, Is.Not.Null);
			Assert.That(bundleFile.IsLoaded);

			// Debug:
			TestContext.WriteLine($"The bundle was loaded by path: {bundleFile.Metadata.Uri.Absolute}.");
		}

		[Test]
		public void InvalidBundle_ShouldThrowException_OnLoad(
			[ValueSource(nameof(BundleSources))] IAssetBundleSource source)
		{
			// Arrange:
			var invalidBundleFile = new AssetBundleFile(AssetBundleUri.Invalid, source);

			// Act:
			void Act() => invalidBundleFile.Load();

			// Assert:
			Assert.That(Act, Throws.InstanceOf<Exception>());
		}

		[UnityTest]
		public IEnumerator LoadAsync_ShouldSucceed([ValueSource(nameof(BundleSources))] IAssetBundleSource source) =>
			ATask.ToCoroutine(async () =>
			{
				// Arrange:
				var bundleFile = new AssetBundleFile(_validUri, source);

				// Act:
				_stopwatch.Restart();
				_loadedBundle = await bundleFile.LoadAsync();
				_stopwatch.Stop();

				// Assert:
				Assert.That(bundleFile.IsLoaded);
				Assert.That(_loadedBundle, Is.Not.Null);
				Assert.IsInstanceOf<AssetBundle>(_loadedBundle);

				// Debug:
				TestContext.WriteLine($"Loaded bundle {_loadedBundle.name} " +
				                      $"by path: {bundleFile.Metadata.Uri.Absolute} " +
				                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

				await Task.Yield();
			});

		[UnityTest]
		public IEnumerator LoadAsync_WithProgress_ShouldSucceed(
			[ValueSource(nameof(BundleSources))] IAssetBundleSource source) =>
			ATask.ToCoroutine(async () =>
			{
				// Arrange:
				var callbackCalls = 0;
				var callbacksCalled = false;
				DownloadProgress lastProgress = default;
				var bundleFile = new AssetBundleFile(_validUri, source);

				// Act:
				_stopwatch.Restart();
				_loadedBundle = await bundleFile.LoadAsync(
					onProgress: progress =>
					{
						callbackCalls++;
						callbacksCalled = true;
						lastProgress = progress;
					});
				_stopwatch.Stop();

				// Assert:
				Assert.That(callbacksCalled);
				Assert.That(callbackCalls, Is.GreaterThan(0));
				Assert.That(lastProgress, Is.EqualTo(DownloadProgress.Full));

				// Debug:
				TestContext.WriteLine("Progress event was called " +
				                      $"{callbackCalls} times " +
				                      $"in {_stopwatch.ElapsedMilliseconds} ms. " +
				                      $"Last value is {lastProgress.NormalizedValue}.");

				await Task.Yield();
			});

		[UnityTest]
		public IEnumerator LoadAsync_CancelBeforeStart_ShouldThrowTaskCanceledException(
			[ValueSource(nameof(BundleSources))] IAssetBundleSource source)
		{
			// Prepare.
			// I do not understand why the bundle does not have time to unload in TearDown.
			AssetBundle.UnloadAllAssetBundles(true);
			yield return null;

			// Arrange:
			var cts = new CancellationTokenSource();
			var bundleFile = new AssetBundleFile(_validUri, source);

			// Act:
			cts.Cancel();
			var loadTask = bundleFile.LoadAsync(cancellationToken: cts.Token);

			// Assert:
			Assert.ThrowsAsync<TaskCanceledException>(async () => await loadTask);
		}

		[UnityTest]
		public IEnumerator LoadAsync_CancelDuringExecution_ShouldThrowTaskCanceledException(
			[ValueSource(nameof(BundleSources))] IAssetBundleSource source)
		{
			// Prepare.
			// I do not understand why the bundle does not have time to unload in TearDown.
			AssetBundle.UnloadAllAssetBundles(true);
			yield return null;

			// Arrange:
			var cts = new CancellationTokenSource();
			var bundleFile = new AssetBundleFile(_validUri, source);

			// Act:
			cts.CancelAfter(1);
			var loadTask = bundleFile.LoadAsync(cancellationToken: cts.Token);
			while (!loadTask.IsCompleted && !loadTask.IsCanceled)
			{
				yield return null;
			}

			// Assert:
			Assert.IsTrue(loadTask.IsCanceled, "Task should be canceled");
			Assert.Throws<TaskCanceledException>(() => loadTask.GetAwaiter().GetResult());
		}

		[UnityTest]
		public IEnumerator SizeOfLoadedAsset_ShouldNotBeZeroOrUnknown(
			[ValueSource(nameof(BundleSources))] IAssetBundleSource source)
		{
			// Arrange:
			var bundleFile = new AssetBundleFile(_validUri, source);
			_loadedBundle = bundleFile.Load();
			yield return null;

			// Act:
			var bundleSize = bundleFile.Metadata.Size;

			// Assert:
			Assert.That(bundleSize, Is.Not.EqualTo(FileSize.Zero));
			Assert.That(bundleSize, Is.Not.EqualTo(FileSize.Unknown));

			// Debug:
			TestContext.WriteLine($"Size of {bundleFile.Metadata.Uri.Relative} is {bundleSize.ToString()}.");

			yield return null;
		}

		[UnityTest]
		public IEnumerator Unload_ShouldSucceed([ValueSource(nameof(BundleSources))] IAssetBundleSource source)
		{
			// Arrange:
			var bundleFile = new AssetBundleFile(_validUri, source);
			bundleFile.Load();
			yield return null;

			// Act:
			bundleFile.Unload();
			yield return null;

			// Assert:
			Assert.That(bundleFile.IsLoaded, Is.False);

			// Debug:
			TestContext.WriteLine($"The bundle with name {bundleFile.Metadata.Uri.Relative} was unloaded.");
		}

		[UnityTest]
		public IEnumerator Dependencies_OfLoadedAsset_ShouldNotBeEmpty(
			[ValueSource(nameof(BundleSources))] IAssetBundleSource source)
		{
			// Arrange:
			var bundleFile = new AssetBundleFile(_validUri, source);
			bundleFile.Load();
			yield return null;

			// Act:
			var dependencies = bundleFile.Dependencies().ToArray();

			// Assert:
			Assert.That(dependencies.Length, Is.GreaterThan(0));

			// Debug:
			TestContext.WriteLine($"Dependencies of {bundleFile.Metadata.Uri.Relative} is {dependencies.Flatten()}.");
		}
	}
}