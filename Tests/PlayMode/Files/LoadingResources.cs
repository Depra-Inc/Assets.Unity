// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Diagnostics;
using System.IO;
using Depra.Assets.Runtime.Files.Database;
using Depra.Assets.Runtime.Files.Resource;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Tests.PlayMode.Mocks;
using Depra.Assets.Tests.PlayMode.Types;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using static Depra.Assets.Runtime.Common.Constants;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(ResourceAsset<>))]
    internal sealed class LoadingResources
    {
        private Stopwatch _stopwatch;
        private TestScriptableAsset _testAsset;
        private TempDirectory _resourcesFolder;
        private CoroutineHostMock _coroutineHost;
        private ResourceAsset<TestScriptableAsset> _resourceAsset;

        [OneTimeSetUp]
        public void SetUp()
        {
            _stopwatch = new Stopwatch();
            _coroutineHost = CoroutineHostMock.Create();
            var assetIdent = new AssetIdent(nameof(TestScriptableAsset), string.Empty);

            // Create resources folder if does not exist.
            var absoluteResourcesPath = Path.Combine(Application.dataPath, ResourcesFolderName);
            _resourcesFolder = new TempDirectory(absoluteResourcesPath);

            // Create a new asset instance.
            var assetNameWithExtension = assetIdent.Name + AssetTypes.BASE;
            var asset = ScriptableObject.CreateInstance<TestScriptableAsset>();
            var fullPath = Path.Combine(AssetsFolderName, ResourcesFolderName, assetNameWithExtension);
            AssetDatabase.CreateAsset(asset, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            _testAsset = asset;

            // Create a new resource asset.
            _resourceAsset = new ResourceAsset<TestScriptableAsset>(assetIdent, _coroutineHost);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_coroutineHost.gameObject);
            
            // Delete the asset.
            var assetPath = AssetDatabase.GetAssetPath(_testAsset);
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _resourcesFolder.DeleteIfEmpty();
        }

        [UnityTest]
        public IEnumerator SingleAssetShouldBeLoaded()
        {
            // Arrange.
            var resourceAsset = _resourceAsset;

            // Act.
            var loadedAsset = resourceAsset.Load();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(resourceAsset.IsLoaded);

            // Debug.
            Debug.Log($"Loaded [{loadedAsset.name}] from {nameof(Resources)}.");

            // Cleanup.
            yield return Free(loadedAsset);
        }

        [UnityTest]
        public IEnumerator MultipleAssetsShouldBeLoadedAndEquals()
        {
            // Arrange.
            var resourceAsset = _resourceAsset;

            // Act.
            var firstLoadedAsset = resourceAsset.Load();
            var secondLoadedAsset = resourceAsset.Load();

            // Assert.
            Assert.That(firstLoadedAsset, Is.Not.Null);
            Assert.That(secondLoadedAsset, Is.Not.Null);
            Assert.That(firstLoadedAsset, Is.EqualTo(secondLoadedAsset));

            // Debug.
            Debug.Log($"Loaded [{firstLoadedAsset.name}] from {nameof(Resources)}.");
            Debug.Log($"Loaded [{secondLoadedAsset.name}] from {nameof(Resources)}.");

            // Cleanup.
            yield return Free(secondLoadedAsset);
        }

        [UnityTest]
        public IEnumerator SingleAssetShouldBeLoadedAsync()
        {
            // Arrange.
            Object loadedAsset = null;
            var resourceAsset = _resourceAsset;

            // Act.
            _stopwatch.Restart();
            var asyncToken = resourceAsset.LoadAsync(
                onLoaded: asset => loadedAsset = asset,
                onFailed: exception => throw exception);

            while (loadedAsset == null)
            {
                yield return null;
            }

            _stopwatch.Stop();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(resourceAsset.IsLoaded);
            Assert.That(asyncToken.IsCanceled, Is.False);

            // Debug.
            Debug.Log($"Loaded [{loadedAsset.name}] " +
                      $"from {nameof(Resources)} " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

            // Cleanup.
            yield return Free(loadedAsset);
        }

        [UnityTest]
        public IEnumerator SingleAssetShouldBeLoadedAsyncWithProgress()
        {
            // Arrange.
            var callbacksCalled = false;
            var callbackCalls = 0;
            Object loadedAsset = null;
            var resourceAsset = _resourceAsset;
            DownloadProgress lastProgress = default;

            // Act.
            _stopwatch.Restart();
            resourceAsset.LoadAsync(
                onLoaded: asset => loadedAsset = asset,
                onProgress: progress =>
                {
                    callbackCalls++;
                    callbacksCalled = true;
                    lastProgress = progress;
                },
                onFailed: exception => throw exception);

            while (resourceAsset.IsLoaded == false)
            {
                yield return null;
            }

            _stopwatch.Stop();

            // Assert.
            Assert.That(callbacksCalled);
            Assert.That(callbackCalls, Is.GreaterThan(0));
            Assert.That(lastProgress, Is.EqualTo(DownloadProgress.Full));

            // Debug.
            Debug.Log("Progress event was called " +
                      $"{callbackCalls} times " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms. " +
                      $"Last value is {lastProgress.NormalizedValue}.");

            // Cleanup.
            yield return Free(loadedAsset);
        }

        [UnityTest]
        public IEnumerator LoadingShouldBeCanceled()
        {
            // Arrange.
            Object loadedAsset = null;
            var resourceAsset = _resourceAsset;

            // Act.
            var asyncToken = resourceAsset.LoadAsync(
                onLoaded: asset => loadedAsset = asset,
                onFailed: exception => throw exception);
            asyncToken.Cancel();

            // Assert.
            Assert.That(loadedAsset, Is.Null);
            Assert.That(asyncToken.IsCanceled);

            // Debug.
            Debug.Log($"Loading of resource [{resourceAsset.Name}] was canceled.");

            // Cleanup.
            yield return Free(loadedAsset);
        }

        [UnityTest]
        public IEnumerator SingleAssetShouldBeUnloaded()
        {
            // Arrange.
            var resourceAsset = _resourceAsset;
            resourceAsset.Load();
            yield return null;

            // Act.
            resourceAsset.Unload();
            yield return null;

            // Assert.
            Assert.That(resourceAsset.IsLoaded, Is.False);

            // Debug.
            Debug.Log($"Loaded and unloaded [{resourceAsset.Name}] from {nameof(Resources)}.");
        }

        [Test]
        public void SingleAssetSizeShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var resourceAsset = _resourceAsset;
            resourceAsset.Load();

            // Act.
            var assetSize = resourceAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Debug.Log($"Size of [{resourceAsset.Name}] is {assetSize.ToHumanReadableString()}.");
        }

        [Test]
        public void SingleInvalidAssetShouldThrowExceptionOnLoad()
        {
            // Arrange.
            var assetIdent = new AssetIdent(string.Empty, string.Empty);
            var invalidResourceAsset = new ResourceAsset<InvalidAsset>(assetIdent);

            // Act.
            void Act() => invalidResourceAsset.Load();

            // Assert.
            Assert.That(Act, Throws.TypeOf<ResourceNotLoadedException>());
        }

        private static IEnumerator Free(Object resourceAsset)
        {
            Resources.UnloadAsset(resourceAsset);
            yield return null;
        }
    }
}