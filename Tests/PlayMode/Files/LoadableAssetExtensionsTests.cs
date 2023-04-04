using System;
using System.Collections;
using System.Linq;
using Depra.Assets.Runtime.Files.Extensions;
using Depra.Assets.Tests.PlayMode.Types;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(LoadableAssetFileExtensions))]
    internal sealed class LoadableAssetExtensionsTests
    {
        private TestScriptableAsset _testAsset;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _testAsset = Resources.LoadAll<TestScriptableAsset>(string.Empty).FirstOrDefault();
            Assert.IsNotNull(_testAsset);
        }

        [UnityTest]
        public IEnumerator TextFromFileShouldBeRead()
        {
            // Arrange.
            var asset = _testAsset;

            // Act.
            
            // Assert.

            throw new NotImplementedException();
        }
    }
}