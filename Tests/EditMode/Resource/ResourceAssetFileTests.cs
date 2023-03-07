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
using TaskExtensions = Depra.Assets.Tests.Common.TaskExtensions;

namespace Depra.Assets.Tests.EditMode.Resource
{
    [TestFixture(TestOf = typeof(ResourceAsset))]
    internal sealed class ResourceAssetFileTests
    {
        private TestAsset _testAsset;
        private AssetFactory _assetFactory;
        private TypedAssetIdent _typedAssetIdent;
        private EditorCoroutineHost _coroutineHost;

        [SetUp]
        public void SetUp()
        {
            _coroutineHost = new EditorCoroutineHost();
            _assetFactory = new ScriptableObjectFactory();
            _testAsset = CreateTestAsset(_assetFactory, ASSET_NAME);
            var assetIdent = new AssetIdent(ASSET_NAME, ASSET_DIRECTORY);
            _typedAssetIdent = new TypedAssetIdent(ASSET_TYPE, assetIdent);
        }

        [TearDown]
        public void TearDown() => _assetFactory.DestroyAsset(_testAsset);

        [Test]
        public void WhenLoadingAResource_ThenTheFileWasLoadedSuccessfully(
            [ValueSource(typeof(Static), nameof(ResourceLoaders))]
            ResourceAssetSyncLoadingStrategy loader)
        {
            // Arrange.
            var resourceAssetFile = new ResourceAsset(_typedAssetIdent, loader, _coroutineHost);

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
            var resourceAssetFile = new ResourceAsset(_typedAssetIdent, loader, _coroutineHost);
            var taskCompletionSource = new TaskCompletionSource<Object>();
            var assetLoadingCallbacks = new AssetLoadingCallbacks(
                onLoaded: loaded => taskCompletionSource.SetResult(loaded),
                onFailed: exception => taskCompletionSource.SetException(exception));

            // Act.
            TaskExtensions.RunAsyncMethodSync(() =>
            {
                return new Task(Load);

                async void Load()
                {
                    resourceAssetFile.LoadAsync(assetLoadingCallbacks);
                    loadedObject = await taskCompletionSource.Task;
                }
            });

            // Assert.
            Assert.NotNull(loadedObject);
        }
    }
}