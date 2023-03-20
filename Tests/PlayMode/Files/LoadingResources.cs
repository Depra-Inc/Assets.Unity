using System.Collections;
using System.Diagnostics;
using System.Linq;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files;
using Depra.Assets.Runtime.Files.Resource;
using Depra.Assets.Runtime.Files.Resource.Exceptions;
using Depra.Assets.Tests.PlayMode.Exceptions;
using Depra.Assets.Tests.PlayMode.Types;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(ResourceAsset<>))]
    internal sealed class LoadingResources
    {
        private Stopwatch _stopwatch;
        private AssetIdent _assetIdent;
        private TestScriptableAsset _testAsset;
        private TestCoroutineHost _coroutineHost;

        private static IEnumerator Free(Object resourceAsset)
        {
            Resources.UnloadAsset(resourceAsset);
            yield return null;
        }

        private static TestScriptableAsset PrepareResourceAssetFile()
        {
            var allTestResources = Resources.LoadAll<TestResourcesRef>(string.Empty);
            var resources = allTestResources.FirstOrDefault() ??
                            throw new TestReferenceNotFoundException(nameof(TestResourcesRef));
            var testAsset = ScriptableObject.CreateInstance<TestScriptableAsset>();
            testAsset.Initialize(resources.AssetName, resources.DirectoryPath);

            return testAsset;
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            _stopwatch = new Stopwatch();
            _testAsset = PrepareResourceAssetFile();
            _assetIdent = _testAsset.Ident;
            _coroutineHost = TestCoroutineHost.Create();
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
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(assetIdent, _coroutineHost);

            // Act.
            var loadedAsset = resourceAsset.Load();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(resourceAsset.IsLoaded);

            // Debug.
            Debug.Log($"Loaded [{loadedAsset.name}] from {nameof(Resources)}.");

            yield return Free(loadedAsset);
        }

        [UnityTest]
        public IEnumerator MultipleAssetsShouldBeLoadedAndEquals()
        {
            // Arrange.
            var assetIdent = _assetIdent;
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(assetIdent, _coroutineHost);

            // Act.
            var firstLoadedAsset = resourceAsset.Load();
            var secondLoadedAsset = resourceAsset.Load();

            // Assert.
            Assert.That(firstLoadedAsset, Is.Not.Null);
            Assert.That(secondLoadedAsset, Is.Not.Null);
            Assert.That(firstLoadedAsset, Is.EqualTo(secondLoadedAsset));

            // Debug.
            Debug.Log($"Loaded [{firstLoadedAsset.name}] from {nameof(Resources)}.");
            Debug.Log($"Loaded [{secondLoadedAsset.name}] from {nameof(Resources)}.");

            yield return Free(secondLoadedAsset);
        }

        [UnityTest]
        public IEnumerator SingleAssetShouldBeLoadedAsync()
        {
            // Arrange.
            var assetIdent = _assetIdent;
            Object loadedAsset = null;
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(assetIdent, _coroutineHost);

            // Act.
            _stopwatch.Restart();
            resourceAsset.LoadAsync(
                onLoaded: asset => loadedAsset = asset,
                onFailed: exception => throw exception);
            
            while (loadedAsset == null)
            {
                yield return null;
            }

            _stopwatch.Stop();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(resourceAsset.IsLoaded);

            // Debug.
            Debug.Log($"Loaded [{loadedAsset.name}] " +
                      $"from {nameof(Resources)} " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.");

            yield return Free(loadedAsset);
        }

        [UnityTest]
        public IEnumerator SingleAssetShouldBeUnloaded()
        {
            // Arrange.
            var assetIdent = _assetIdent;
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(assetIdent, _coroutineHost);
            resourceAsset.Load();
            yield return null;

            // Act.
            resourceAsset.Unload();
            yield return null;

            // Assert.
            Assert.That(resourceAsset.IsLoaded, Is.False);

            // Debug.
            Debug.Log($"Loaded and unloaded [{assetIdent.Name}] from {nameof(Resources)}.");
        }

        [Test]
        public void SingleAssetSizeShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var assetIdent = _assetIdent;
            var resourceAsset = new ResourceAsset<TestScriptableAsset>(assetIdent, _coroutineHost);
            resourceAsset.Load();

            // Act.
            var assetSize = resourceAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Debug.Log($"Size of [{assetIdent.Name}] is {assetSize.ToHumanReadableString()}.");
        }

        [Test]
        public void SingleInvalidAssetShouldThrowExceptionOnLoad()
        {
            // Arrange.
            var assetIdent = new AssetIdent(string.Empty, string.Empty);
            var resourceAsset = new ResourceAsset<InvalidAsset>(assetIdent);

            // Act.
            void Act() => resourceAsset.Load();

            // Assert.
            Assert.That(Act, Throws.TypeOf<ResourceNotLoadedException>());
        }
    }
}