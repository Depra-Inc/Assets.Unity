using System;
using Depra.Assets.Runtime.Factory;
using Depra.Assets.Tests.Common;
using NUnit.Framework;

namespace Depra.Assets.Tests.EditMode
{
    [TestFixture(TestOf = typeof(ScriptableObjectFactory))]
    internal sealed class ScriptableObjectFactoryTests
    {
        private const string ASSET_NAME = "TestAsset";
        private const string ASSET_DIRECTORY = "Resources";
        private static readonly Type ASSET_TYPE = typeof(TestAsset);

        [Test]
        public void Can_Create_Asset_And_Destroy_After()
        {
            var assetFactory = new ScriptableObjectFactory();
            var createdAsset = assetFactory.CreateAsset(ASSET_TYPE, ASSET_DIRECTORY, ASSET_NAME);

            Assert.IsNotNull(createdAsset);
            
            assetFactory.DestroyAsset(createdAsset);
            
            Assert.IsTrue(createdAsset == null);
        }

        [Test]
        public void Can_Create_Asset_Using_Generic_Method_And_Destroy_After()
        {
            var assetFactory = new ScriptableObjectFactory();
            var createdAsset = assetFactory.CreateAsset<TestAsset>(ASSET_DIRECTORY, ASSET_NAME);

            Assert.IsNotNull(createdAsset);
            
            assetFactory.DestroyAsset(createdAsset);

            Assert.IsTrue(createdAsset == null);
        }
    }
}