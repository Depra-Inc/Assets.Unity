// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Database;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Resource;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Tests.PlayMode.Mocks;
using Depra.Assets.Tests.PlayMode.Types;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using static Depra.Assets.Runtime.Common.Constants;
using static UnityEngine.Debug;
using Assert = NUnit.Framework.Assert;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(ResourceAsset<>))]
    internal sealed class LoadingResources
    {
        private readonly Stack<TestScriptableAsset> _loadedAssets = new();
        
        private Stopwatch _stopwatch;
        private AssetIdent _assetIdent;
        private TempDirectory _resourcesFolder;
        private TestScriptableAsset _testAsset;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _stopwatch = new Stopwatch();
            _assetIdent = new AssetIdent(nameof(TestScriptableAsset));

            // Create resources folder if does not exist.
            var absoluteResourcesPath = Path.Combine(Application.dataPath, RESOURCES_FOLDER_NAME);
            _resourcesFolder = new TempDirectory(absoluteResourcesPath);

            // Create a new asset instance.
            var assetNameWithExtension = _assetIdent.Name + AssetTypes.BASE;
            var asset = ScriptableObject.CreateInstance<TestScriptableAsset>();
            var fullPath = Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME, assetNameWithExtension);
            AssetDatabase.CreateAsset(asset, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            _testAsset = asset;
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

            _resourcesFolder.DeleteIfEmpty();
        }

        [Test]
        public void SingleAssetShouldBeLoaded()
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_assetIdent);

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
        public void MultipleAssetsShouldBeLoadedAndEquals()
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_assetIdent);

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
        public IEnumerator SingleAssetShouldBeLoadedAsync() => UniTask.ToCoroutine(async () =>
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_assetIdent);

            // Act.
            _stopwatch.Restart();
            var loadedAsset = await resourceAsset.LoadAsync(CancellationToken.None);
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
        public IEnumerator SingleAssetShouldBeLoadedAsyncWithProgress() => UniTask.ToCoroutine(async () =>
        {
            // Arrange.
            var callbackCalls = 0;
            var callbacksCalled = false;
            DownloadProgress lastProgress = default;
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_assetIdent);

            // Act.
            _stopwatch.Restart();
            var loadedAsset = await resourceAsset.LoadAsync(
                CancellationToken.None,
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
            Log("Progress event was called " +
                $"{callbackCalls} times " +
                $"in {_stopwatch.ElapsedMilliseconds} ms. " +
                $"Last value is {lastProgress.NormalizedValue}.");

            // Cleanup.
            _loadedAssets.Push(loadedAsset);
        });

        [UnityTest]
        public IEnumerator LoadingShouldBeCanceled() => UniTask.ToCoroutine(async () =>
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_assetIdent);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Act.
            var loadingTask = resourceAsset.LoadAsync(cancellationToken);
            await UniTask.Yield();
            cancellationTokenSource.Cancel();

            // Assert.
            Assert.That(cancellationToken.IsCancellationRequested);
            Assert.That(loadingTask.Status == UniTaskStatus.Canceled);

            // Debug.
            Log($"Loading of resource {resourceAsset.Name} was canceled.");

            await UniTask.Yield();
        });

        [UnityTest]
        public IEnumerator SingleAssetShouldBeUnloaded()
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_assetIdent);
            resourceAsset.Load();
            yield return null;

            // Act.
            resourceAsset.Unload();
            yield return null;

            // Assert.
            Assert.That(resourceAsset.IsLoaded, Is.False);

            // Debug.
            Log($"{resourceAsset.Name} unloaded from {nameof(Resources)}.");
        }

        [Test]
        public void SingleAssetSizeShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(_assetIdent);
            resourceAsset.Load();

            // Act.
            var assetSize = resourceAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Log($"Size of {resourceAsset.Name} is {assetSize.ToHumanReadableString()}.");
        }

        [Test]
        public void SingleInvalidAssetShouldThrowExceptionOnLoad()
        {
            // Arrange.
            var assetIdent = new AssetIdent(string.Empty);
            var invalidResourceAsset = new ResourceAsset<InvalidAsset>(assetIdent);

            // Act.
            void Act() => invalidResourceAsset.Load();

            // Assert.
            Assert.That(Act, Throws.TypeOf<ResourceNotLoadedException>());
        }
    }
}