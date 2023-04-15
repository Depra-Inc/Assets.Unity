// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using Depra.Assets.Editor.Files;
using Depra.Assets.Runtime.Async.Tokens;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Tests.PlayMode.Types;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Debug;
using Assert = NUnit.Framework.Assert;
using Object = UnityEngine.Object;

namespace Depra.Assets.Tests.EditMode.Files
{
    [TestFixture(TestOf = typeof(PreloadedAsset<>))]
    internal sealed class LoadingPreloadedAssets
    {
        private Object _testInstance;
        private InvalidAsset _invalidAsset;
        private Object[] _initialPreloadedAssets;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _invalidAsset = new InvalidAsset();
        }

        [SetUp]
        public void Setup()
        {
            _initialPreloadedAssets = PlayerSettings.GetPreloadedAssets();
            _testInstance = ScriptableObject.CreateInstance<TestScriptableAsset>();
            PlayerSettings.SetPreloadedAssets(new[] { _testInstance });
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_testInstance);
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
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(preloadedAsset.IsLoaded);

            // Debug.
            Log($"{nameof(TestScriptableAsset)} loaded from {nameof(PlayerSettings)}.");
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
            Assert.That(preloadedAsset.IsLoaded, Is.False);

            // Debug.
            Log($"{preloadedAsset.Name} unloaded from {nameof(PlayerSettings)}.");
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
            Assert.That(firstLoadedAsset, Is.Not.Null);
            Assert.That(secondLoadedAsset, Is.Not.Null);
            Assert.That(firstLoadedAsset, Is.EqualTo(secondLoadedAsset));

            // Debug.
            Log($"{firstLoadedAsset.name} loaded from {nameof(PlayerSettings)}.");
        }

        [Test]
        public void SingleAssetShouldBeLoadedAsync()
        {
            // Arrange.
            Object loadedAsset = null;
            var preloadedAsset = new PreloadedAsset<TestScriptableAsset>(_invalidAsset);

            // Act.
            preloadedAsset.LoadAsync(
                onLoaded: asset => loadedAsset = asset,
                onFailed: exception => throw exception);

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(preloadedAsset.IsLoaded);

            // Debug.
            Log($"{loadedAsset.name} loaded from {nameof(PlayerSettings)}.");
        }

        [Test]
        public void AssetSizeShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var preloadedAsset = new PreloadedAsset<TestScriptableAsset>(_invalidAsset);
            preloadedAsset.Load();

            // Act.
            var assetSize = preloadedAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Log($"Size of {preloadedAsset.Name} is {assetSize.ToHumanReadableString()}.");
        }

        private sealed class InvalidAsset : ILoadableAsset<TestScriptableAsset>
        {
            public string Name =>
                nameof(TestScriptableAsset);

            public string Path =>
                throw new NotImplementedException();

            public bool IsLoaded =>
                throw new NotImplementedException();

            public FileSize Size => FileSize.Zero;

            public TestScriptableAsset Load() =>
                throw new NotImplementedException();

            public void Unload() { }

            public IAsyncToken LoadAsync(Action<TestScriptableAsset> onLoaded,
                Action<DownloadProgress> onProgress = null, Action<Exception> onFailed = null) =>
                throw new NotImplementedException();
        }
    }
}