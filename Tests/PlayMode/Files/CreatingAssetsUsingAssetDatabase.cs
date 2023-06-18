// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Files.Database;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using static Depra.Assets.Runtime.Common.Constants;
using static UnityEngine.Debug;
using Assert = NUnit.Framework.Assert;

namespace Depra.Assets.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(DatabaseAsset<>))]
    internal sealed class CreatingAssetsUsingAssetDatabase
    {
        private const string ASSET_NAME = "TestAsset";
        private const string ASSET_TYPE_EXTENSION = ".asset";

        private Stopwatch _stopwatch;
        private DatabaseAssetIdent _assetIdent;

        [SetUp]
        public void Setup()
        {
            _stopwatch = new Stopwatch();
            _assetIdent = new DatabaseAssetIdent(RESOURCES_FOLDER_NAME, ASSET_NAME, ASSET_TYPE_EXTENSION);
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(_assetIdent.AbsolutePath);
        }

        [UnityTest]
        [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
        public IEnumerator SingleAssetShouldBeLoadedAsync() => UniTask.ToCoroutine(async () =>
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<ScriptableAsset>(_assetIdent);

            // Act.
            _stopwatch.Restart();
            var loadedAsset = await databaseAsset.LoadAsync();
            _stopwatch.Stop();

            // Assert.
            Assert.That(databaseAsset.IsLoaded);
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.IsInstanceOf<ScriptableAsset>(loadedAsset);

            // Debug.
            Log($"{loadedAsset.name} loaded " +
                $"from {nameof(AssetDatabase)} " +
                $"in {_stopwatch.ElapsedMilliseconds} ms.");
        });

        private sealed class ScriptableAsset : ScriptableObject { }
    }
}