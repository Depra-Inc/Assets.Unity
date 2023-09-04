// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Extensions;
using Depra.Assets.Runtime.Files.Resource;
using Depra.Assets.Tests.PlayMode.Stubs;
using Depra.Assets.Tests.PlayMode.Utils;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Depra.Assets.Tests.PlayMode.Files
{
	[TestFixture(TestOf = typeof(ResourcesAsset<>))]
	internal sealed class ResourcesAssetTests
	{
		private const int CANCEL_DELAY = 1000;
		private const string ASSET_EXTENSION = AssetTypes.BASE;
		private const string ASSET_NAME = nameof(PlayModeTestScriptableAsset);

		private readonly Stack<PlayModeTestScriptableAsset> _loadedAssets = new();

		private Stopwatch _stopwatch;
		private ResourcesPath _resourcesIdent;
		private PlayModeTestScriptableAsset _testAsset;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_stopwatch = new Stopwatch();
			_resourcesIdent = new ResourcesPath(name: ASSET_NAME, extension: ASSET_EXTENSION);
			_resourcesIdent.Directory.CreateIfNotExists();
			_testAsset = TestEnvironment.CreateAsset<PlayModeTestScriptableAsset>(_resourcesIdent.ProjectPath);
		}

		[TearDown]
		public void Teardown()
		{
			foreach (var loadedAsset in _loadedAssets)
			{
				Resources.UnloadAsset(loadedAsset);
			}

			_loadedAssets.Clear();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (TestEnvironment.TryDeleteAsset(_testAsset))
			{
				_resourcesIdent.Directory.DeleteIfEmpty();
			}
		}

		[Test]
		public void Load_ShouldSucceed()
		{
			// Arrange.
			var resourceAsset = new ResourcesAsset<PlayModeTestScriptableAsset>(_resourcesIdent);

			// Act.
			var loadedAsset = resourceAsset.Load();

			// Assert.
			Assert.That(loadedAsset, Is.Not.Null);
			Assert.That(resourceAsset.IsLoaded);

			// Debug.
			TestContext.WriteLine($"{loadedAsset.name} loaded from {nameof(Resources)}.");

			// Cleanup.
			_loadedAssets.Push(loadedAsset);
		}

		[Test]
		public void LoadMultiple_ShouldSucceed()
		{
			// Arrange.
			var resourceAsset = new ResourcesAsset<PlayModeTestScriptableAsset>(_resourcesIdent);

			// Act.
			var firstLoadedAsset = resourceAsset.Load();
			var secondLoadedAsset = resourceAsset.Load();

			// Assert.
			Assert.That(firstLoadedAsset, Is.Not.Null);
			Assert.That(secondLoadedAsset, Is.Not.Null);
			Assert.That(firstLoadedAsset, Is.EqualTo(secondLoadedAsset));

			// Debug.
			TestContext.WriteLine($"{firstLoadedAsset.name} loaded from {nameof(Resources)}.");
			TestContext.WriteLine($"{secondLoadedAsset.name} loaded from {nameof(Resources)}.");

			// Cleanup.
			_loadedAssets.Push(firstLoadedAsset);
			_loadedAssets.Push(secondLoadedAsset);
		}

		[UnityTest]
		public IEnumerator LoadAsync_ShouldSucceed() => ATask.ToCoroutine(async () =>
		{
			// Arrange.
			var resourceAsset = new ResourcesAsset<PlayModeTestScriptableAsset>(_resourcesIdent);
			var cancellationToken = new CancellationTokenSource(CANCEL_DELAY).Token;

			// Act.
			_stopwatch.Restart();
			var loadedAsset = await resourceAsset.LoadAsync(cancellationToken: cancellationToken);
			_stopwatch.Stop();

			// Assert.
			Assert.That(resourceAsset.IsLoaded);
			Assert.That(loadedAsset, Is.Not.Null);
			Assert.IsInstanceOf<PlayModeTestScriptableAsset>(loadedAsset);

			// Debug.
			TestContext.WriteLine($"{loadedAsset.name} loaded " +
			                      $"from {nameof(Resources)} " +
			                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

			// Cleanup.
			_loadedAssets.Push(loadedAsset);
		});

		[UnityTest]
		public IEnumerator LoadAsync_WithProgress_ShouldSucceed() => ATask.ToCoroutine(async () =>
		{
			// Arrange.
			var callbackCalls = 0;
			var callbacksCalled = false;
			DownloadProgress lastProgress = default;
			var resourceAsset = new ResourcesAsset<PlayModeTestScriptableAsset>(_resourcesIdent);

			// Act.
			_stopwatch.Restart();
			var loadedAsset = await resourceAsset.LoadAsync(
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

			// Cleanup.
			_loadedAssets.Push(loadedAsset);
		});

		[Test]
		public void LoadAsync_CancelBeforeStart_ShouldThrowTaskCanceledException()
		{
			// Arrange.
			var cts = new CancellationTokenSource();
			var resourceAsset = new ResourcesAsset<PlayModeTestScriptableAsset>(_resourcesIdent);

			// Act.
			cts.Cancel();
			var loadTask = resourceAsset.LoadAsync(cancellationToken: cts.Token);

			// Assert.
			Assert.ThrowsAsync<TaskCanceledException>(async () => await loadTask);
		}

		[UnityTest]
		public IEnumerator LoadAsync_CancelDuringExecution_ShouldThrowTaskCanceledException()
		{
			// Arrange.
			var cts = new CancellationTokenSource();
			var resourceAsset = new ResourcesAsset<PlayModeTestScriptableAsset>(_resourcesIdent);

			// Act.
			cts.CancelAfter(1);
			var loadTask = resourceAsset.LoadAsync(cancellationToken: cts.Token);
			async Task Act() => await loadTask;

			yield return new WaitUntil(() => cts.Token.IsCancellationRequested);

			// Assert.
			Assert.ThrowsAsync<TaskCanceledException>(Act);
		}

		[UnityTest]
		public IEnumerator Unload_ShouldSucceed()
		{
			// Arrange.
			var resourceAsset = new ResourcesAsset<PlayModeTestScriptableAsset>(_resourcesIdent);
			resourceAsset.Load();
			yield return null;

			// Act.
			resourceAsset.Unload();
			yield return null;

			// Assert.
			Assert.That(resourceAsset.IsLoaded, Is.False);

			// Debug.
			TestContext.WriteLine($"{resourceAsset.Ident.RelativeUri} unloaded from {nameof(Resources)}.");
		}

		[Test]
		public void SizeOfLoadedAsset_ShouldNotBeZeroOrUnknown()
		{
			// Arrange.
			var resourceAsset = new ResourcesAsset<PlayModeTestScriptableAsset>(_resourcesIdent);
			resourceAsset.Load();

			// Act.
			var assetSize = resourceAsset.Size;

			// Assert.
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

			// Debug.
			TestContext.WriteLine($"Size of {resourceAsset.Ident.RelativeUri} is {assetSize.ToHumanReadableString()}.");
		}

		[UnityTest]
		public IEnumerator SizeOfAsyncLoadedAsset_ShouldNotBeZeroOrUnknown() => ATask.ToCoroutine(async () =>
		{
			// Arrange.
			var resourceAsset = new ResourcesAsset<PlayModeTestScriptableAsset>(_resourcesIdent);
			await resourceAsset.LoadAsync();

			// Act.
			var assetSize = resourceAsset.Size;

			// Assert.
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
			Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

			// Debug.
			TestContext.WriteLine($"Size of {resourceAsset.Ident.RelativeUri} is {assetSize.ToHumanReadableString()}.");
		});
	}
}