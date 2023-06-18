// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Group;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.ValueObjects;
using Depra.Assets.Tests.PlayMode.Stubs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.Debug;
using Assert = NUnit.Framework.Assert;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(AssetGroup))]
    internal sealed class LoadingFileGroups
    {
        private const int GROUP_SIZE = 3;

        private Stopwatch _stopwatch;
        private NamedAssetIdent _testNamedAssetIdent;
        private List<ILoadableAsset<Object>> _testAssets;

        [SetUp]
        public void Setup()
        {
            _stopwatch = new Stopwatch();
            _testNamedAssetIdent = NamedAssetIdent.Empty;
            _testAssets = new List<ILoadableAsset<Object>>(GROUP_SIZE);
            for (var index = 0; index < GROUP_SIZE; index++)
            {
                _testAssets.Add(new FakeAssetFile());
            }
        }

        [Test]
        public void GroupShouldBeLoaded()
        {
            // Arrange.
            var assetGroup = new AssetGroup(_testNamedAssetIdent, children: _testAssets);

            // Act.
            var loadedAssets = assetGroup.Load().ToArray();

            // Assert.
            Assert.That(loadedAssets, Is.Not.Null);
            Assert.That(loadedAssets, Is.Not.Empty);

            // Debug.
            Log($"Loaded asset group with {_testAssets.Count} children.");
        }

        [UnityTest]
        public IEnumerator GroupShouldBeLoadedAsync() => UniTask.ToCoroutine(async () =>
        {
            // Arrange.
            var resourceAsset = new AssetGroup(_testNamedAssetIdent, children: _testAssets);

            // Act.
            _stopwatch.Restart();
            var loadedAssets = await resourceAsset.LoadAsync(cancellationToken: CancellationToken.None);
            loadedAssets = loadedAssets.ToArray();
            _stopwatch.Stop();

            // Assert.
            Assert.That(loadedAssets, Is.Not.Null);
            Assert.That(loadedAssets, Is.Not.Empty);

            // Debug.
            Log($"Loaded asset group with {_testAssets.Count} children " +
                $"in {_stopwatch.ElapsedMilliseconds} ms.");
        });

        [UnityTest]
        public IEnumerator GroupShouldBeLoadedAsyncWithProgress() => UniTask.ToCoroutine(async () =>
        {
            // Arrange.
            var callbackCalls = 0;
            var callbacksCalled = false;
            DownloadProgress lastProgress = default;
            var assetGroup = new AssetGroup(_testNamedAssetIdent, children: _testAssets);

            // Act.
            _stopwatch.Restart();
            await assetGroup.LoadAsync(
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
        });

        [Test]
        public void GroupShouldBeUnloaded()
        {
            // Arrange.
            var resourceAsset = new AssetGroup(_testNamedAssetIdent, children: _testAssets);
            var unused = resourceAsset.Load();

            // Act.
            resourceAsset.Unload();

            // Assert.
            Assert.That(resourceAsset.IsLoaded, Is.False);

            // Debug.
            Log("Asset group unloaded.");
        }

        [Test]
        public void GroupSizeShouldBeThreeBytes()
        {
            // Arrange.
            Assert.That(_testAssets.Sum(x => x.Size.SizeInBytes), Is.EqualTo(3));
            var assetGroup = new AssetGroup(_testNamedAssetIdent, children: _testAssets);
            var unused = assetGroup.Load();

            // Act.
            var assetSize = assetGroup.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Cleanup.
            Log($"Size of {assetGroup.Ident.RelativeUri} is {assetSize.ToHumanReadableString()}.");
        }
    }
}