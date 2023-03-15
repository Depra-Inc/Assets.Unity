using System.Collections;
using System.Diagnostics;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Resource;
using Depra.Assets.Tests.Common;
using Depra.Assets.Tests.PlayMode.Utils;
using Depra.Coroutines.Unity.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode
{
    [TestFixture(TestOf = typeof(ResourceAsset<>))]
    internal sealed class LoadingResources
    {
        private Stopwatch _stopwatch;
        private TestAsset _testAsset;
        private AssetIdent _assetIdent;
        private RuntimeCoroutineHost _coroutineHost;

        private static IEnumerator Free(Object resourceAsset)
        {
            Resources.UnloadAsset(resourceAsset);
            yield return null;
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            _stopwatch = new Stopwatch();
            _coroutineHost = new GameObject().AddComponent<RuntimeCoroutineHost>();
            _testAsset = Create.ResourceAssetFile();
            _assetIdent = _testAsset.Ident;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_coroutineHost.gameObject);
        }

        [UnityTest]
        public IEnumerator SingleAssetShouldBeLoaded()
        {
            // Arrange.
            var assetIdent = _assetIdent;
            var resourceAsset = new ResourceAsset<TestAsset>(assetIdent, _coroutineHost);

            // Act.
            var loadedAsset = resourceAsset.Load();
            Debug.Log($"Loaded [{assetIdent.Name}] from resources.");

            // Assert.
            Assert.IsNotNull(loadedAsset);
            Assert.IsTrue(resourceAsset.IsLoaded);

            yield return Free(loadedAsset);
        }

        [UnityTest]
        public IEnumerator MultipleAssetsShouldBeLoadedAndEquals()
        {
            // Arrange.
            var assetIdent = _assetIdent;
            var resourceAsset = new ResourceAsset<TestAsset>(_assetIdent, _coroutineHost);

            // Act.
            var firstLoadedAsset = resourceAsset.Load();
            var secondLoadedAsset = resourceAsset.Load();
            Debug.Log($"Loaded [{assetIdent.Name}] from resources.");

            // Assert.
            Assert.IsNotNull(firstLoadedAsset);
            Assert.IsNotNull(secondLoadedAsset);
            Assert.AreEqual(firstLoadedAsset, secondLoadedAsset);

            yield return Free(secondLoadedAsset);
        }

        [UnityTest]
        public IEnumerator SingleAssetShouldBeLoadedAsync()
        {
            // Arrange.
            var assetIdent = _assetIdent;
            Object loadedAsset = null;
            var resourceAsset = new ResourceAsset<TestAsset>(assetIdent, _coroutineHost);
            var assetLoadingCallbacks = new AssetLoadingCallbacks<TestAsset>(
                onLoaded: asset => loadedAsset = asset,
                onFailed: exception => throw exception);

            // Act.
            _stopwatch.Restart();
            resourceAsset.LoadAsync(assetLoadingCallbacks);
            while (loadedAsset == null)
            {
                yield return null;
            }

            _stopwatch.Stop();
            Debug.Log($"Loaded [{assetIdent.Name}] from resources in {_stopwatch.ElapsedMilliseconds} ms.");

            // Assert.
            Assert.NotNull(loadedAsset);
            Assert.IsTrue(resourceAsset.IsLoaded);

            yield return Free(loadedAsset);
        }

        [UnityTest]
        public IEnumerator SingleAssetShouldBeUnloaded()
        {
            // Arrange.
            var assetIdent = _assetIdent;
            var resourceAsset = new ResourceAsset<TestAsset>(assetIdent, _coroutineHost);
            resourceAsset.Load();
            yield return null;

            // Act.
            resourceAsset.Unload();
            yield return null;
            Debug.Log($"Loaded and unloaded [{assetIdent.Name}] from resources");

            // Assert.
            Assert.IsFalse(resourceAsset.IsLoaded);
        }
    }
}