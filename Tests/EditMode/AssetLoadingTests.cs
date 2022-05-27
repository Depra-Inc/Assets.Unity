using System;
using System.IO;
using Depra.Assets.Runtime.Factory;
using Depra.Assets.Runtime.Loading.Resource.Sync;
using NUnit.Framework;

namespace Depra.Assets.Tests.EditMode
{
    public class AssetLoadingTests
    {
        private static readonly Type AssetType = typeof(TestAsset);
        private const string AssetDirectory = "";
        private const string AssetName = "TestAsset";

        private AssetFactory _assetFactory;
        private TestAsset _testAsset;

        [SetUp]
        public void SetUp()
        {
            _testAsset = CreateTestAsset();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyTestAsset();
        }

        [Test]
        public void Can_Load_Asset_From_Resources()
        {
            var assetLoader = new ResourceLoader();
            var loadedAsset = assetLoader.LoadAsset(AssetType, Path.Combine(AssetDirectory, AssetName));

            Assert.IsNotNull(loadedAsset);
        }

        [Test]
        public void Can_Load_Asset_From_Resources_Using_Generic_Method()
        {
            var assetLoader = new ResourceLoader();
            var loadedAsset = assetLoader.LoadAsset<TestAsset>(Path.Combine(AssetDirectory, AssetName));

            Assert.IsNotNull(loadedAsset);
        }

        private TestAsset CreateTestAsset()
        {
            _assetFactory = new ScriptableObjectFactory();
            var fullDirectory = Path.Combine("Resources", AssetDirectory);
            return _assetFactory.CreateAsset<TestAsset>(fullDirectory, AssetName);
        }

        private void DestroyTestAsset()
        {
            _assetFactory.DestroyAsset(_testAsset);
            _assetFactory = null;
        }
    }
}