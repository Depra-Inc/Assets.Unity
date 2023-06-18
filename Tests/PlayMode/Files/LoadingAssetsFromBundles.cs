// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.ValueObjects;
using Depra.Assets.Tests.PlayMode.Mocks;
using Depra.Assets.Tests.PlayMode.Types;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.Debug;
using Assert = NUnit.Framework.Assert;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(AssetBundleAssetFile<>))]
    internal sealed class LoadingAssetsFromBundles
    {
        private const string TEST_BUNDLE_NAME = "test";
        private const string TEST_ASSET_NAME = "TestAsset";

        private Stopwatch _stopwatch;
        private AssetBundle _assetBundle;
        private AssetBundleAssetFile<TestScriptableAsset> _assetFromBundle;

        [SetUp]
        public void Setup()
        {
            var assetBundlesDirectory = new TestAssetBundlesDirectory(GetType());
            var assetBundlePath = Path.Combine(assetBundlesDirectory.AbsolutePath, TEST_BUNDLE_NAME);
            _assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            var assetIdent = new FileSystemAssetIdent(TEST_ASSET_NAME, assetBundlePath);
            _assetFromBundle = new AssetBundleAssetFile<TestScriptableAsset>(assetIdent, _assetBundle);
        }

        [TearDown]
        public void TearDown()
        {
            _assetFromBundle.Unload();
            _assetBundle.Unload(true);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _stopwatch = new Stopwatch();
        }

        [Test]
        public void AssetFromBundleShouldBeLoaded()
        {
            // Arrange.
            var bundle = _assetBundle;
            var bundleAsset = _assetFromBundle;

            // Act.
            var loadedAsset = bundleAsset.Load();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(bundleAsset.IsLoaded);

            // Debug.
            Log($"{loadedAsset.name} loaded from bundle with name: {bundle.name}.");
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeUnloaded()
        {
            // Arrange.
            var bundle = _assetBundle;
            var bundleAsset = _assetFromBundle;
            bundleAsset.Load();
            yield return null;

            // Act.
            bundleAsset.Unload();
            yield return null;

            // Assert.
            Assert.That(bundleAsset.IsLoaded, Is.False);

            // Debug.
            Log($"{bundleAsset.Ident.RelativeUri} unloaded from bundle with name: {bundle.name}.");
        }

        [Test]
        public void InvalidAssetFromBundleShouldThrowExceptionOnLoad()
        {
            // Arrange.
            var bundle = _assetBundle;
            var invalidAssetIdent = FileSystemAssetIdent.Invalid;
            var invalidAssetFromBundle = new AssetBundleAssetFile<InvalidAsset>(invalidAssetIdent, bundle);

            // Act.
            void Act() => invalidAssetFromBundle.Load();

            // Assert.
            Assert.That(Act, Throws.TypeOf<AssetBundleFileNotLoadedException>());
        }

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeLoadedAsync() => UniTask.ToCoroutine(async () =>
        {
            // Arrange.
            var assetFromBundle = _assetFromBundle;

            // Act.
            _stopwatch.Restart();
            var loadedAsset = await assetFromBundle.LoadAsync(cancellationToken: CancellationToken.None);
            _stopwatch.Stop();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.IsInstanceOf<TestScriptableAsset>(loadedAsset);

            // Debug.
            Log($"{loadedAsset.name} loaded " +
                $"from bundle {_assetBundle.name} " +
                $"in {_stopwatch.ElapsedMilliseconds} ms.");
        });

        [UnityTest]
        public IEnumerator AssetFromBundleShouldBeLoadedAsyncWithProgress() => UniTask.ToCoroutine(async () =>
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
            Log("Progress event was called " +
                $"{callbackCalls} times " +
                $"in {_stopwatch.ElapsedMilliseconds} ms. " +
                $"Last value is {lastProgress.NormalizedValue}.");
        });

        [Test]
        public void AssetFromBundleSizeShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var bundleAsset = _assetFromBundle;
            bundleAsset.Load();

            // Act.
            var assetSize = bundleAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Log($"Size of {bundleAsset.Ident.RelativeUri} is {assetSize.ToHumanReadableString()}.");
        }
    }
}