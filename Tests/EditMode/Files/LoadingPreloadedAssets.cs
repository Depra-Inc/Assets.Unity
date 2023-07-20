﻿// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Unity.Editor.Files;
using Depra.Assets.Unity.Runtime.Files.Adapter;
using Depra.Assets.Unity.Tests.EditMode.Stubs;
using Depra.Assets.Unity.Tests.PlayMode.Stubs;
using Depra.Assets.ValueObjects;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Debug;
using Assert = NUnit.Framework.Assert;

namespace Depra.Assets.Unity.Tests.EditMode.Files
{
    [TestFixture(TestOf = typeof(PreloadedAsset<>))]
    internal sealed class LoadingPreloadedAssets
    {
        private const int CANCEL_DELAY = 1000;

        private Object _testInstance;
        private Object[] _initialPreloadedAssets;
        private IUnityLoadableAsset<TestScriptableAsset> _childAsset;

        [OneTimeSetUp]
        public void OneTimeSetup() => 
            _childAsset = new FakeAssetFile(new FakeAssetIdent(nameof(TestScriptableAsset)));

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
        public void Load_ShouldSucceed()
        {
            // Arrange.
            var preloadedAsset = new PreloadedAsset<TestScriptableAsset>(_childAsset);

            // Act.
            var loadedAsset = preloadedAsset.Load();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(preloadedAsset.IsLoaded);

            // Debug.
            Log($"{nameof(TestScriptableAsset)} loaded from {nameof(PlayerSettings)}.");
        }

        [Test]
        public void LoadMultiple_ShouldSucceed()
        {
            // Arrange.
            var resourceAsset = new PreloadedAsset<TestScriptableAsset>(_childAsset);

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
        public void LoadAsync_ShouldSucceed() => UniTask.Void(async () =>
        {
            // Arrange.
            var preloadedAsset = new PreloadedAsset<TestScriptableAsset>(_childAsset);
            var cancellationToken = new CancellationTokenSource(CANCEL_DELAY).Token;

            // Act.
            var loadedAsset = await preloadedAsset.LoadAsync(cancellationToken: cancellationToken);

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(preloadedAsset.IsLoaded);

            // Debug.
            Log($"{loadedAsset.name} loaded from {nameof(PlayerSettings)}.");
        });

        [Test]
        public void Unload_ShouldSucceed()
        {
            // Arrange.
            var preloadedAsset = new PreloadedAsset<TestScriptableAsset>(_childAsset);
            preloadedAsset.Load();

            // Act.
            preloadedAsset.Unload();

            // Assert.
            Assert.That(preloadedAsset.IsLoaded, Is.False);

            // Debug.
            Log($"{preloadedAsset.Ident.RelativeUri} unloaded from {nameof(PlayerSettings)}.");
        }

        [Test]
        public void SizeOfLoadedAsset_ShouldNotBeZeroOrUnknown()
        {
            // Arrange.
            var preloadedAsset = new PreloadedAsset<TestScriptableAsset>(_childAsset);
            preloadedAsset.Load();

            // Act.
            var assetSize = preloadedAsset.Size;

            // Assert.
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Zero));
            Assert.That(assetSize, Is.Not.EqualTo(FileSize.Unknown));

            // Debug.
            Log($"Size of {preloadedAsset.Ident.RelativeUri} is {assetSize.ToHumanReadableString()}.");
        }
    }
}