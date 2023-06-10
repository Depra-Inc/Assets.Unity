// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Bundles.IO;
using Depra.Assets.Runtime.Files.Bundles.Memory;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Tests.PlayMode.Mocks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.Debug;
using Assert = NUnit.Framework.Assert;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(AssetBundleFile))]
    internal sealed class LoadingAssetBundles
    {
        private const string TEST_BUNDLE_NAME = "test";

        private Stopwatch _stopwatch;
        private AssetBundle _loadedBundle;

        private static IEnumerable<AssetBundleFile> AllBundles()
        {
            var sourceType = typeof(TestAssetBundlesDirectory);
            var assetBundlesDirectory = new TestAssetBundlesDirectory(sourceType);
            var bundleIdent = new FileSystemAssetIdent(TEST_BUNDLE_NAME, assetBundlesDirectory.AbsolutePath);

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

        [OneTimeSetUp]
        public void OneTimeSetup() => _stopwatch = new Stopwatch();
        
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
        public void BundleShouldBeLoaded([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile)
        {
            // Arrange.

            // Act.
            _loadedBundle = bundleFile.Load();

            // Assert.
            Assert.That(_loadedBundle, Is.Not.Null);
            Assert.That(bundleFile.IsLoaded);

            // Debug.
            Log($"The bundle was loaded by path: {bundleFile.Path}.");
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
        public IEnumerator BundleShouldBeLoadedAsync([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile) =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange.

                // Act.
                _stopwatch.Restart();
                _loadedBundle = await bundleFile.LoadAsync(CancellationToken.None);
                _stopwatch.Stop();

                // Assert.
                Assert.That(bundleFile.IsLoaded);
                Assert.That(_loadedBundle, Is.Not.Null);
                Assert.IsInstanceOf<AssetBundle>(_loadedBundle);

                // Debug.
                Log($"Loaded bundle {_loadedBundle.name} " +
                    $"by path: {bundleFile.Path} " +
                    $"in {_stopwatch.ElapsedMilliseconds} ms.");

                await UniTask.Yield();
            });

        [UnityTest]
        public IEnumerator BundleShouldBeLoadedWithProgress(
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
                
                await UniTask.Yield();
            });

        [UnityTest]
        public IEnumerator BundleLoadingShouldBeCanceled([ValueSource(nameof(AllBundles))] AssetBundleFile bundleFile) =>
            UniTask.ToCoroutine(async () =>
            {
                // Arrange.
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;

                // Act.
                _loadedBundle = await bundleFile.LoadAsync(cancellationToken);
                cancellationTokenSource.Cancel();

                // Assert.
                Assert.That(_loadedBundle, Is.Null);
                Assert.That(cancellationToken.IsCancellationRequested);

                // Debug.
                Log($"Loading of bundle {bundleFile.Name} was canceled.");

                await UniTask.Yield();
            });

        [UnityTest]
        public IEnumerator BundleSizeShouldNotBeZeroOrUnknown(
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
            Log($"Size of {bundleFile.Name} is {bundleSize.ToHumanReadableString()}.");

            yield return null;
        }
    }
}