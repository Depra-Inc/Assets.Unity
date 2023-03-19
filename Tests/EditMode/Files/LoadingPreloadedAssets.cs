using System;
using System.Linq;
using Depra.Assets.Editor.Files;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Files;
using Depra.Assets.Tests.PlayMode.Types;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Tests.EditMode.Files
{
    [TestFixture(TestOf = typeof(PreloadedAsset<>))]
    internal sealed class LoadingPreloadedAssets
    {
        private TestScriptableAsset _testAsset;
        private InvalidAsset _invalidAsset;
        private Object[] _initialPreloadedAssets;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _invalidAsset = new InvalidAsset();
            _testAsset = Resources.LoadAll<TestScriptableAsset>(string.Empty).FirstOrDefault();
            Assert.IsNotNull(_testAsset);
        }

        [SetUp]
        public void Setup()
        {
            _initialPreloadedAssets = PlayerSettings.GetPreloadedAssets();
            PlayerSettings.SetPreloadedAssets(new Object[] { _testAsset });
        }

        [TearDown]
        public void TearDown()
        {
            PlayerSettings.SetPreloadedAssets(_initialPreloadedAssets);
        }
        
        [Test]
        public void SingleAssetShouldBeLoaded()
        {
            // Arrange.
            var preloadedAsset = new PreloadedAsset<TestScriptableAsset>(_invalidAsset);

            // Act.
            var loadedAsset = preloadedAsset.Load();

            // Assert.
            Assert.IsNotNull(loadedAsset);
            Assert.IsTrue(preloadedAsset.IsLoaded);

            // Debug.
            Debug.Log($"Loaded preloaded [{nameof(TestScriptableAsset)}] from {nameof(PlayerSettings)}.");
        }

        [Test]
        public void SingleAssetShouldBeUnloaded()
        {
            // Arrange.
            var preloadedAsset = new PreloadedAsset<TestScriptableAsset>(_invalidAsset);
            preloadedAsset.Load();

            // Act.
            preloadedAsset.Unload();

            // Assert.
            Assert.IsFalse(preloadedAsset.IsLoaded);

            // Debug.
            Debug.Log($"Loaded and unloaded [{preloadedAsset.Name}] from {nameof(PlayerSettings)}.");
        }

        [Test]
        public void MultipleAssetsShouldBeLoadedAndEquals()
        {
            // Arrange.
            var resourceAsset = new PreloadedAsset<TestScriptableAsset>(_invalidAsset);

            // Act.
            var firstLoadedAsset = resourceAsset.Load();
            var secondLoadedAsset = resourceAsset.Load();

            // Assert.
            Assert.IsNotNull(firstLoadedAsset);
            Assert.IsNotNull(secondLoadedAsset);
            Assert.AreEqual(firstLoadedAsset, secondLoadedAsset);

            // Debug.
            Debug.Log($"Loaded preloaded [{firstLoadedAsset.name}] from {nameof(PlayerSettings)}.");
            Debug.Log($"Loaded preloaded [{secondLoadedAsset.name}] from {nameof(PlayerSettings)}.");
        }

        [Test]
        public void SingleAssetShouldBeLoadedAsync()
        {
            // Arrange.
            Object loadedAsset = null;
            var preloadedAsset = new PreloadedAsset<TestScriptableAsset>(_invalidAsset);
            var assetLoadingCallbacks = new AssetLoadingCallbacks<TestScriptableAsset>(
                onLoaded: asset => loadedAsset = asset,
                onFailed: exception => throw exception);

            // Act.
            preloadedAsset.LoadAsync(assetLoadingCallbacks);

            // Assert.
            Assert.NotNull(loadedAsset);
            Assert.IsTrue(preloadedAsset.IsLoaded);

            // Debug.
            Debug.Log($"Loaded preloaded [{loadedAsset.name}] from {nameof(PlayerSettings)}.");
        }

        private sealed class InvalidAsset : ILoadableAsset<TestScriptableAsset>
        {
            public string Name => nameof(TestScriptableAsset);
            public string Path => throw new NotImplementedException();

            public FileSize Size => FileSize.Zero;
            public bool IsLoaded => throw new NotImplementedException();

            public TestScriptableAsset Load() => throw new NotImplementedException();

            public void Unload() { }

            public IDisposable LoadAsync(IAssetLoadingCallbacks<TestScriptableAsset> callbacks) =>
                throw new NotImplementedException();
        }
    }
}