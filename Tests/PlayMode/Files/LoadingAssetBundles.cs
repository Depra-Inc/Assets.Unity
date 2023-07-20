// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Unity.Runtime.Files.Bundles.Files;
using Depra.Assets.Unity.Runtime.Files.Bundles.IO;
using Depra.Assets.Unity.Runtime.Files.Bundles.Memory;
using Depra.Assets.Unity.Runtime.Files.Idents;
using Depra.Assets.Unity.Tests.PlayMode.Stubs;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.Debug;
using Assert = NUnit.Framework.Assert;

namespace Depra.Assets.Unity.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(AssetBundleFile))]
    internal sealed class LoadingAssetBundles
    {
        private const string TEST_BUNDLE_NAME = "test";

        private static IEnumerable<AssetBundleFile> AllBundles()
        {
            var assetBundlesDirectory = new TestAssetBundlesDirectory(typeof(TestAssetBundlesDirectory));
            var bundleIdent = new FileSystemAssetIdent(assetBundlesDirectory.AbsolutePath, TEST_BUNDLE_NAME);

            yield return new AssetBundleFromFile(bundleIdent);
            yield return new AssetBundleFromMemory(bundleIdent);
            yield return new AssetBundleFromStream(bundleIdent);
            //yield return new AssetBundleFromWeb(bundleIdent);
        }

        private static IEnumerable<AssetBundleFile> InvalidBundles()
        {
            var invalidIdent = FileSystemAssetIdent.Invalid;
            yield return new AssetBundleFromFile(invalidIdent);
            yield return new AssetBundleFromMemory(invalidIdent);
            yield return new AssetBundleFromStream(invalidIdent);
            //yield return new AssetBundleFromWeb(invalidIdent);
        }

        private Stopwatch _stopwatch;
        private AssetBundle _loadedBundle;

        [OneTimeSetUp]
        public void OneTimeSetup() =>
            _stopwatch = new Stopwatch();

        [TearDown]
        public void TearDown()
        {
            if (_loadedBundle == null)
            {
                return;
            }

            _loadedBundle.Unload(true);
            _loadedBundle = null;
        }

        [Test]
        public void Load_ShouldSucceed([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange & Act.
            _loadedBundle = bundleFile.Load();

            // Assert.
            Assert.That(_loadedBundle, Is.Not.Null);
            Assert.That(bundleFile.IsLoaded);

            // Debug.
            Log($"The bundle was loaded by path: {bundleFile.Ident.Uri}.");
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
                Log($"Loaded bundle {_loadedBundle.name} " +
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
                Log("Progress event was called " +
                    $"{callbackCalls} times " +
                    $"in {_stopwatch.ElapsedMilliseconds} ms. " +
                    $"Last value is {lastProgress.NormalizedValue}.");

                await UniTask.Yield();
            });

        [UnityTest]
        public IEnumerator BundleLoadingShouldBeCanceled([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile) =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange.
                var cancellationTokenSource = new CancellationTokenSource();

                // Act.
                _loadedBundle = await bundleFile.LoadAsync(cancellationToken: cancellationTokenSource.Token);
                cancellationTokenSource.Cancel();

                // Assert.
                Assert.That(_loadedBundle, Is.Null);

                // Debug.
                Log($"Loading of bundle {bundleFile.Ident.RelativeUri} was canceled.");

                await UniTask.Yield();
            });

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
            Log($"Size of {bundleFile.Ident.RelativeUri} is {bundleSize.ToHumanReadableString()}.");

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
            Log($"The bundle with name {bundleFile.Ident.RelativeUri} was unloaded.");
        }
    }
}