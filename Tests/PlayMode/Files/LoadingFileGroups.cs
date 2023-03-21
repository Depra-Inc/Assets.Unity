using System;
using System.Collections.Generic;
using System.Linq;
using Depra.Assets.Runtime.Async.Tokens;
using Depra.Assets.Runtime.Files;
using Depra.Assets.Tests.PlayMode.Types;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(AssetGroup))]
    internal sealed class LoadingFileGroups
    {
        private const int GROUP_SIZE = 3;

        private List<ILoadableAsset<Object>> _testAssets;

        [SetUp]
        public void Setup()
        {
            _testAssets = new List<ILoadableAsset<Object>>(GROUP_SIZE);
            for (var index = 0; index < GROUP_SIZE; index++)
            {
                _testAssets.Add(new FakeAsset());
            }
        }

        [Test]
        public void AssetGroupShouldBeLoaded()
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

        [Test]
        public void AssetShouldBeLoadedAsync()
        {
            // Arrange.
            Object[] loadedAssets = null;
            var resourceAsset = new AssetGroup(children: _testAssets);

            // Act.
            resourceAsset.LoadAsync(
                onLoaded: asset => loadedAssets = asset.ToArray(),
                onFailed: exception => throw exception);

            // Assert.
            Assert.That(loadedAssets, Is.Not.Null);
            Assert.That(loadedAssets, Is.Not.Empty);
        }

        [Test]
        public void AssetGroupShouldBeUnloaded()
        {
            // Arrange.
            var resourceAsset = new AssetGroup(children: _testAssets);
            var unused = resourceAsset.Load();

            // Act.
            resourceAsset.Unload();

            // Assert.
            Assert.That(resourceAsset.IsLoaded, Is.False);
        }

        private sealed class FakeAsset : ILoadableAsset<TestScriptableAsset>
        {
            public FakeAsset()
            {
                Size = FileSize.Zero;
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

                return AsyncActionToken.Empty;
            }

            public void Unload() => IsLoaded = false;
        }
    }
}