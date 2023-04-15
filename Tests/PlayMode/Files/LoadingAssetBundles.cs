// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Bundles.IO;
using Depra.Assets.Runtime.Files.Bundles.Memory;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Tests.PlayMode.Mocks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.Debug;
using Assert = NUnit.Framework.Assert;
using Object = UnityEngine.Object;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(AssetBundleFile))]
    internal sealed class LoadingAssetBundles
    {
        private const string TEST_BUNDLE_NAME = "test";

        private static CoroutineHostMock _coroutineHost;
        private Stopwatch _stopwatch;

        private static CoroutineHostMock CoroutineHost =>
            _coroutineHost ??= CoroutineHostMock.Create();

        private static IEnumerable<AssetBundleFile> AllBundles()
        {
            var sourceType = typeof(TestAssetBundlesDirectory);
            var assetBundlesDirectory = new TestAssetBundlesDirectory(sourceType);
            var bundleIdent = new AssetIdent(TEST_BUNDLE_NAME, assetBundlesDirectory.AbsolutePath);

            yield return new AssetBundleFromFile(bundleIdent, CoroutineHost);
            yield return new AssetBundleFromMemory(bundleIdent, CoroutineHost);
            yield return new AssetBundleFromStream(bundleIdent, CoroutineHost);
            //yield return new AssetBundleFromWeb(bundleIdent, CoroutineHost);
        }

        private static IEnumerable<AssetBundleFile> InvalidBundles()
        {
            var invalidBundleIdent = new AssetIdent("InvalidBundle", "InvalidPath");

            yield return new AssetBundleFromFile(invalidBundleIdent, CoroutineHost);
            yield return new AssetBundleFromMemory(invalidBundleIdent, CoroutineHost);
            yield return new AssetBundleFromStream(invalidBundleIdent, CoroutineHost);
            //yield return new AssetBundleFromWeb(bundleIdent, CoroutineHost);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _stopwatch = new Stopwatch();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Object.DestroyImmediate(_coroutineHost.gameObject);
        }

        [UnityTest]
        public IEnumerator BundleShouldBeLoaded([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.

            // Act.
            var loadedAssetBundle = bundleFile.Load();

            // Assert.
            Assert.That(loadedAssetBundle, Is.Not.Null);
            Assert.That(bundleFile.IsLoaded);

            // Debug.
            Log($"The bundle was loaded by path: {bundleFile.Path}.");

            // Cleanup.
            loadedAssetBundle.Unload(true);
            yield return null;
        }

        [UnityTest]
        public IEnumerator BundleShouldBeUnloaded([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
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
            Log($"The bundle with name {bundleFile.Name} was unloaded.");
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
        public IEnumerator BundleShouldBeLoadedAsync([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.
            AssetBundle loadedBundle = null;

            // Act.
            _stopwatch.Restart();
            var asyncToken = bundleFile.LoadAsync(
                onLoaded: asset => loadedBundle = asset,
                onFailed: exception => throw exception);

            while (loadedBundle == null)
            {
                yield return null;
            }

            _stopwatch.Stop();

            // Assert.
            Assert.That(loadedBundle, Is.Not.Null);
            Assert.That(bundleFile.IsLoaded);
            Assert.That(asyncToken.IsCanceled, Is.False);

            // Debug.
            Log($"Loaded bundle {loadedBundle.name} " +
                      $"by path: {bundleFile.Path} " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

            // Cleanup.
            loadedBundle.Unload(true);
            yield return null;
        }

        [UnityTest]
        public IEnumerator BundleShouldBeLoadedWithProgress(
            [ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.
            var callbacksCalled = false;
            var callbackCalls = 0;
            AssetBundle loadedBundle = null;
            DownloadProgress lastProgress = default;

            // Act.
            _stopwatch.Restart();
            bundleFile.LoadAsync(
                onLoaded: asset => loadedBundle = asset,
                onProgress: progress =>
                {
                    callbackCalls++;
                    callbacksCalled = true;
                    lastProgress = progress;
                },
                onFailed: exception => throw exception);

            while (bundleFile.IsLoaded == false)
            {
                yield return null;
            }

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
            loadedBundle.Unload(true);
            yield return null;
        }

        [UnityTest]
        public IEnumerator BundleLoadingShouldBeCanceled([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.
            AssetBundle loadedBundle = null;

            // Act.
            var asyncToken = bundleFile.LoadAsync(
                onLoaded: asset => loadedBundle = asset,
                onFailed: exception => throw exception);
            asyncToken.Cancel();

            // Assert.
            Assert.That(loadedBundle, Is.Null);
            Assert.That(asyncToken.IsCanceled);

            // Debug.
            Log($"Loading of bundle {bundleFile.Name} was canceled.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BundleSizeShouldNotBeZeroOrUnknown(
            [ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.
            var loadedBundle = bundleFile.Load();
            yield return null;

            // Act.
            var bundleSize = bundleFile.Size;

            // Assert.
            Assert.That(bundleSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(bundleSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Log($"Size of {bundleFile.Name} is {bundleSize.ToHumanReadableString()}.");

            // Cleanup.
            loadedBundle.Unload(true);
            yield return null;
        }
    }
}