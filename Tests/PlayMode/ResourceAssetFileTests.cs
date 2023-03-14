using System.Collections;
using System.Diagnostics;
using System.IO;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Factory;
using Depra.Assets.Runtime.Resource.Files;
using Depra.Assets.Runtime.Utils;
using Depra.Assets.Tests.Common;
using Depra.Coroutines.Domain.Entities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode
{
    [TestFixture(TestOf = typeof(ResourceAsset<>))]
    internal sealed class ResourceAssetFileTests
    {
        private const string ASSET_DIRECTORY = "";
        private const string ASSET_NAME = "TestAsset";
        private const string RESOURCES_FOLDER_NAME = "Resources";

        private Stopwatch _stopwatch;
        private TestAsset _testAsset;
        private AssetIdent _assetIdent;
        private AssetFactory _assetFactory;
        private ICoroutineHost _coroutineHost;

        [SetUp]
        public void SetUp()
        {
            _stopwatch = new Stopwatch();
            _coroutineHost = AssetCoroutineHook.Instance;
            _assetFactory = new ScriptableObjectFactory();
            _testAsset = CreateTestAsset(_assetFactory, ASSET_NAME);
            _assetIdent = new AssetIdent(ASSET_NAME, ASSET_DIRECTORY);
        }

        [TearDown]
        public void TearDown()
        {
            AssetCoroutineHook.Destroy();
            _assetFactory.DestroyAsset(_testAsset);
        }
        
        [Test]
        public void WhenLoadingAResource_ThenTheFileWasLoadedSuccessfully()
        {
            // Arrange.
            var resourceAssetFile = new ResourceAsset<TestAsset>(_assetIdent, _coroutineHost);

            // Act.
            var loadedResource = resourceAssetFile.Load();

            // Assert.
            Assert.IsNotNull(loadedResource);
        }
        
        [UnityTest]
        public IEnumerator WhenLoadingAResourceAsync_ThenTheFileWasLoadedSuccessfully()
        {
            // Arrange.
            Object loadedResource = null;
            var resourceAssetFile = new ResourceAsset<TestAsset>(_assetIdent, _coroutineHost);
            var assetLoadingCallbacks = new AssetLoadingCallbacks<TestAsset>(
                onLoaded: asset => loadedResource = asset,
                onFailed: exception => throw exception);

            // Act.
            _stopwatch.Restart();
            resourceAssetFile.LoadAsync(assetLoadingCallbacks);
            while (loadedResource == null)
            {
                yield return null;
            }
            
            _stopwatch.Stop();
            Debug.Log($"Duration = {_stopwatch.ElapsedMilliseconds}");

            // Assert.
            Assert.NotNull(loadedResource);
        }

        private static TestAsset CreateTestAsset(AssetFactory assetFactory, string assetName)
        {
            var absolutePath = Path.Combine(RESOURCES_FOLDER_NAME, ASSET_DIRECTORY);
            return assetFactory.CreateAsset<TestAsset>(absolutePath, assetName);
        }
    }
}