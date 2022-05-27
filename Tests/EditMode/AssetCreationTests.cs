using System;
using Depra.Assets.Runtime.Factory;
using NUnit.Framework;

namespace Depra.Assets.Tests.EditMode
{
    public class AssetCreationTests
    {
        private static readonly Type AssetType = typeof(TestAsset);
        private const string AssetDirectory = "Resources";
        private const string AssetName = "TestAsset";
        
        [Test]
        public void Can_Create_Asset_And_Destroy_After()
        {
            var assetFactory = new ScriptableObjectFactory();
            var createdAsset = assetFactory.CreateAsset(AssetType, AssetDirectory, AssetName);

            Assert.IsNotNull(createdAsset);
            
            assetFactory.DestroyAsset(createdAsset);
            
            Assert.IsTrue(createdAsset == null);
        }

        [Test]
        public void Can_Create_Asset_Using_Generic_Method_And_Destroy_After()
        {
            var assetFactory = new ScriptableObjectFactory();
            var createdAsset = assetFactory.CreateAsset<TestAsset>(AssetDirectory, AssetName);

            Assert.IsNotNull(createdAsset);
            
            assetFactory.DestroyAsset(createdAsset);

            Assert.IsTrue(createdAsset == null);
        }
    }
}