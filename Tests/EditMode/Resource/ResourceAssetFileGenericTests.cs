using System.Threading.Tasks;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Factory;
using Depra.Assets.Runtime.Resource.Files;
using Depra.Assets.Runtime.Resource.Loading;
using Depra.Assets.Tests.Common;
using Depra.Coroutines.Editor;
using NUnit.Framework;
using UnityEngine;
using static Depra.Assets.Tests.EditMode.Resource.Static;

namespace Depra.Assets.Tests.EditMode.Resource
{
    [TestFixture(TestOf = typeof(ResourceAsset<>))]
    internal sealed class ResourceAssetFileGenericTests
    {
        private TestAsset _testAsset;
        private AssetIdent _assetIdent;
        private AssetFactory _assetFactory;
        private EditorCoroutineHost _coroutineHost;

        [SetUp]
        public void SetUp()
        {
            _coroutineHost = new EditorCoroutineHost();
            _assetFactory = new ScriptableObjectFactory();
            _testAsset = CreateTestAsset(_assetFactory, ASSET_NAME);
            _assetIdent = new AssetIdent(ASSET_NAME, ASSET_DIRECTORY);
        }

        [TearDown]
        public void TearDown() =>
            _assetFactory.DestroyAsset(_testAsset);

        [Test]
        public void WhenLoadingAResource_ThenTheFileWasLoadedSuccessfully(
            [ValueSource(typeof(Static), nameof(ResourceLoaders))]
            ResourceAssetSyncLoadingStrategy loader)
        {
            // Arrange.
            var resourceAssetFile = new ResourceAsset<TestAsset>(_assetIdent, loader, _coroutineHost);

            // Act.
            var loadedAsset = resourceAssetFile.Load();

            // Assert.
            Assert.IsNotNull(loadedAsset);
        }

        [Test]
        public void WhenLoadingAResourceAsync_ThenTheFileWasLoadedSuccessfully(
            [ValueSource(typeof(Static), nameof(ResourceLoaders))]
            ResourceAssetSyncLoadingStrategy loader)
        {
            // Arrange.
            Object loadedObject = null;
            var resourceAssetFile = new ResourceAsset<TestAsset>(_assetIdent, loader, _coroutineHost);
            var taskCompletionSource = new TaskCompletionSource<Object>();
            var assetLoadingCallbacks = new AssetLoadingCallbacks<TestAsset>(
                onLoaded: loaded => taskCompletionSource.SetResult(loaded),
                onFailed: exception => taskCompletionSource.SetException(exception));

            // Act.
            resourceAssetFile.LoadAsync(assetLoadingCallbacks);
            Task.Run(async () => { loadedObject = await taskCompletionSource.Task; }).Wait();

            // Assert.
            Assert.NotNull(loadedObject);
        }
    }
}