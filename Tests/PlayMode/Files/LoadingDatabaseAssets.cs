// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Depra.Assets.Unity.Runtime.Common;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Database;
using Depra.Assets.Unity.Tests.PlayMode.Stubs;
using NUnit.Framework;
using UnityEngine;
using static Depra.Assets.Unity.Runtime.Common.Constants;
using static Depra.Assets.Unity.Runtime.Common.Paths;

namespace Depra.Assets.Unity.Tests.PlayMode.Files
{
    [TestFixture(TestOf = typeof(DatabaseAsset<>))]
    internal sealed class LoadingDatabaseAssets
    {
        private const string TESTS_FOLDER_NAME = "Tests";
        private const string ASSET_NAME = nameof(TestScriptableAsset);

        private Stopwatch _stopwatch;
        private DatabaseAssetIdent _assetIdent;

        [SetUp]
        public void Setup()
        {
            _stopwatch = new Stopwatch();
            _assetIdent = new DatabaseAssetIdent(
                relativeDirectory: Path.Combine(PACKAGES_FOLDER_NAME, FullModuleName, TESTS_FOLDER_NAME, ASSETS_FOLDER_NAME),
                name: ASSET_NAME,
                extension: AssetTypes.BASE);
        }
        
        [Test]
        public void LoadAsync_ShouldThrowsAssetCanNotBeLoadedException()
        {
            // Arrange.
            var databaseAsset = new DatabaseAsset<ScriptableAsset>(_assetIdent);

            // Act.
            async Task Act() => await databaseAsset.LoadAsync();

            // Assert.
            Assert.ThrowsAsync<AssetCanNotBeLoaded>(Act);
        }

        private sealed class ScriptableAsset : ScriptableObject { }
    }
}