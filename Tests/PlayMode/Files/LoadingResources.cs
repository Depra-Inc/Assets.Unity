﻿// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Depra.Assets.Unity.Runtime.Common;
using Depra.Assets.Unity.Runtime.Files.Resource;
using Depra.Assets.Unity.Tests.PlayMode.Stubs;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.Debug;
using Assert = NUnit.Framework.Assert;

namespace Depra.Assets.Unity.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(ResourceAsset<>))]
    internal sealed class LoadingResources
    {
        private const int CANCEL_DELAY = 1000;
        private const string ASSET_EXTENSION = AssetTypes.BASE;
        private const string ASSET_NAME = nameof(TestScriptableAsset);

        private readonly Stack<TestScriptableAsset> _loadedAssets = new();

        private Stopwatch _stopwatch;
        private ResourcesPath _resourceIdent;
        private TestScriptableAsset _testAsset;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _stopwatch = new Stopwatch();
            _resourceIdent = new ResourcesPath(name: ASSET_NAME, extension: ASSET_EXTENSION);
            // Create resources folder if does not exist.
            //_resourceIdent.Directory.CreateIfNotExists();

            // Create a new asset instance.
            _testAsset = ScriptableObject.CreateInstance<TestScriptableAsset>();
            AssetDatabase.CreateAsset(_testAsset, _resourceIdent.ProjectPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
            // Delete the asset.
            var assetPath = AssetDatabase.GetAssetPath(_testAsset);
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //_resourceIdent.Directory.DeleteIfEmpty();
        }

        [Test]
        public void Load_ShouldSucceed()
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_resourceIdent);

            // Act.
            var loadedAsset = resourceAsset.Load();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(resourceAsset.IsLoaded);

            // Debug.
            Log($"{loadedAsset.name} loaded from {nameof(Resources)}.");

            // Cleanup.
            _loadedAssets.Push(loadedAsset);
        }

        [Test]
        public void LoadMultiple_ShouldSucceed()
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_resourceIdent);

            // Act.
            var firstLoadedAsset = resourceAsset.Load();
            var secondLoadedAsset = resourceAsset.Load();

            // Assert.
            Assert.That(firstLoadedAsset, Is.Not.Null);
            Assert.That(secondLoadedAsset, Is.Not.Null);
            Assert.That(firstLoadedAsset, Is.EqualTo(secondLoadedAsset));

            // Debug.
            Log($"{firstLoadedAsset.name} loaded from {nameof(Resources)}.");
            Log($"{secondLoadedAsset.name} loaded from {nameof(Resources)}.");

            // Cleanup.
            _loadedAssets.Push(firstLoadedAsset);
            _loadedAssets.Push(secondLoadedAsset);
        }

        [UnityTest]
        public IEnumerator LoadAsync_ShouldSucceed() => UniTask.ToCoroutine(async () =>
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_resourceIdent);
            var cancellationToken = new CancellationTokenSource(CANCEL_DELAY).Token;

            // Act.
            _stopwatch.Restart();
            var loadedAsset = await resourceAsset.LoadAsync(cancellationToken: cancellationToken);
            _stopwatch.Stop();

            // Assert.
            Assert.That(resourceAsset.IsLoaded);
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.IsInstanceOf<TestScriptableAsset>(loadedAsset);

            // Debug.
            Log($"{loadedAsset.name} loaded " +
                $"from {nameof(Resources)} " +
                $"in {_stopwatch.ElapsedMilliseconds} ms.");

            // Cleanup.
            _loadedAssets.Push(loadedAsset);
        });

        [UnityTest]
        public IEnumerator LoadAsync_WithProgress_ShouldSucceed() => UniTask.ToCoroutine(async () =>
        {
            // Arrange.
            var callbackCalls = 0;
            var callbacksCalled = false;
            DownloadProgress lastProgress = default;
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_resourceIdent);

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
            Log("Progress event was called " +
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
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_resourceIdent);

            // Act.
            cts.Cancel();
            var loadingOperation = resourceAsset.LoadAsync(cancellationToken: cts.Token);

            // Assert.
            Assert.ThrowsAsync<TaskCanceledException>(async () => await loadingOperation);
        }

        [UnityTest]
        public IEnumerator LoadAsync_CancelDuringExecution_ShouldThrowTaskCanceledException()
        {
            // Arrange.
            var cts = new CancellationTokenSource();
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_resourceIdent);

            // Act.
            cts.CancelAfterSlim(TimeSpan.MinValue);
            var loadTask = resourceAsset.LoadAsync(cancellationToken: cts.Token);

            yield return null;

            // Assert.
            Assert.ThrowsAsync<TaskCanceledException>(async () => { await loadTask; });
        }

        [UnityTest]
        public IEnumerator Unload_ShouldSucceed()
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_resourceIdent);
            resourceAsset.Load();
            yield return null;

            // Act.
            resourceAsset.Unload();
            yield return null;

            // Assert.
            Assert.That(resourceAsset.IsLoaded, Is.False);

            // Debug.
            Log($"{resourceAsset.Ident.RelativeUri} unloaded from {nameof(Resources)}.");
        }

        [Test]
        public void SizeOfLoadedAsset_ShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_resourceIdent);
            resourceAsset.Load();

            // Act.
            var assetSize = resourceAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Log($"Size of {resourceAsset.Ident.RelativeUri} is {assetSize.ToHumanReadableString()}.");
        }

        [UnityTest]
        public IEnumerator SizeOfAsyncLoadedAsset_ShouldNotBeZeroOrUnknown() => UniTask.ToCoroutine(async () =>
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_resourceIdent);
            await resourceAsset.LoadAsync();

            // Act.
            var assetSize = resourceAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Log($"Size of {resourceAsset.Ident.RelativeUri} is {assetSize.ToHumanReadableString()}.");
        });
    }
}