using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Depra.Assets.Runtime.Async.Tokens;
using Depra.Assets.Runtime.Files.Group;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Tests.PlayMode.Types;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(AssetGroup))]
    internal sealed class LoadingFileGroups
    {
        private const int GROUP_SIZE = 3;

        private Stopwatch _stopwatch;
        private List<ILoadableAsset<Object>> _testAssets;

        [SetUp]
        public void Setup()
        {
            _stopwatch = new Stopwatch();
            _testAssets = new List<ILoadableAsset<Object>>(GROUP_SIZE);
            for (var index = 0; index < GROUP_SIZE; index++)
            {
                _testAssets.Add(new FakeAsset());
            }
        }

        [Test]
        public void GroupShouldBeLoaded()
        {
            // Arrange.
            var allAssets = _testAssets;
            var assetGroup = new AssetGroup(children: allAssets);

            // Act.
            var loadedAssets = assetGroup.Load().ToArray();

            // Assert.
            Assert.That(loadedAssets, Is.Not.Null);
            Assert.That(loadedAssets, Is.Not.Empty);
        }

        [UnityTest]
        public IEnumerator GroupShouldBeLoadedAsync()
        {
            // Arrange.
            Object[] loadedAssets = null;
            var resourceAsset = new AssetGroup(children: _testAssets);

            // Act.
            _stopwatch.Restart();
            var asyncToken = resourceAsset.LoadAsync(
                onLoaded: assets => loadedAssets = assets.ToArray(),
                onFailed: exception => throw exception);

            while (loadedAssets == null)
            {
                yield return null;
            }

            _stopwatch.Stop();

            // Assert.
            Assert.That(loadedAssets, Is.Not.Null);
            Assert.That(loadedAssets, Is.Not.Empty);
            Assert.That(asyncToken.IsCanceled, Is.False);

            // Debug.
            Debug.Log($"Loaded asset group with {_testAssets.Count} children " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");
        }

        [UnityTest]
        public IEnumerator GroupShouldBeLoadedAsyncWithProgress()
        {
            // Arrange.
            var callbacksCalled = false;
            var callbackCalls = 0;
            var allAssets = _testAssets;
            var assetGroup = new AssetGroup(children: allAssets);

            // Act.
            _stopwatch.Restart();
            assetGroup.LoadAsync(
                onLoaded: _ => { },
                onProgress: _ =>
                {
                    callbackCalls++;
                    callbacksCalled = true;
                },
                onFailed: exception => throw exception);

            while (assetGroup.IsLoaded == false)
            {
                yield return null;
            }

            _stopwatch.Stop();

            // Assert.
            Assert.That(callbacksCalled);
            Assert.That(callbackCalls, Is.GreaterThan(0));

            // Debug.
            Debug.Log("Progress event was called " +
                      $"{callbackCalls} times " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");
        }

        [Test]
        public void GroupShouldBeUnloaded()
        {
            // Arrange.
            var resourceAsset = new AssetGroup(children: _testAssets);
            var unused = resourceAsset.Load();

            // Act.
            resourceAsset.Unload();

            // Assert.
            Assert.That(resourceAsset.IsLoaded, Is.False);
        }

        [Test]
        [SuppressMessage("ReSharper", "IteratorMethodResultIsIgnored")]
        public void GroupSizeShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            Assert.That(_testAssets.All(x => !Equals(x.Size, FileSize.Zero)));
            var assetGroup = new AssetGroup(nameof(AssetGroup), children: _testAssets);
            assetGroup.Load();

            // Act.
            var assetSize = assetGroup.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Cleanup.
            Debug.Log($"Size of [{assetGroup.Name}] is {assetSize.ToHumanReadableString()}.");
        }

        private sealed class FakeAsset : ILoadableAsset<TestScriptableAsset>
        {
            public FakeAsset()
            {
                Size = new FileSize(1);
                Name = nameof(FakeAsset);
                Path = Name;
            }

            public string Name { get; }
            public string Path { get; }

            public FileSize Size { get; }
            public bool IsLoaded { get; private set; }

            private static TestScriptableAsset CreateAsset() =>
                ScriptableObject.CreateInstance<TestScriptableAsset>();

            public TestScriptableAsset Load()
            {
                IsLoaded = true;
                return CreateAsset();
            }

            public IAsyncToken LoadAsync(Action<TestScriptableAsset> onLoaded, Action<float> onProgress = null,
                Action<Exception> onFailed = null)
            {
                var asset = CreateAsset();
                onProgress?.Invoke(1f);
                onLoaded.Invoke(asset);
                IsLoaded = true;

                return AsyncActionToken.Empty;
            }

            public void Unload() => IsLoaded = false;
        }
    }
}