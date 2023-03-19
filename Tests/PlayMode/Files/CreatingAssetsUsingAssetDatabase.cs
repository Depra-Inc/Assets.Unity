using System.Collections;
using System.Diagnostics;
using System.IO;
using Depra.Assets.Runtime.Abstract.Loading;
using Depra.Assets.Runtime.Files;
using Depra.Assets.Runtime.Files.Database;
using Depra.Assets.Runtime.Utils;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using static Depra.Assets.Runtime.Common.Constants;
using Debug = UnityEngine.Debug;

namespace Depra.Assets.Tests.PlayMode.Files
{
    internal sealed class CreatingAssetsUsingAssetDatabase
    {
        private const string ASSET_NAME = "TestAsset";
        private const string ASSET_TYPE_EXTENSION = ".asset";
        private const string ASSET_DIRECTORY_NAME = RESOURCES_FOLDER_NAME;

        private Stopwatch _stopwatch;
        private string _absoluteAssetPath;
        private UnityMainThreadDispatcher _mainThreadDispatcher;

        [SetUp]
        public void Setup()
        {
            _stopwatch = new Stopwatch();
            _mainThreadDispatcher = new GameObject().AddComponent<UnityMainThreadDispatcher>();
            _absoluteAssetPath = Path.Combine(ASSETS_FOLDER_NAME, ASSET_DIRECTORY_NAME, ASSET_NAME + ASSET_TYPE_EXTENSION);
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(_absoluteAssetPath);
            Object.DestroyImmediate(_mainThreadDispatcher.gameObject);
        }

        [UnityTest]
        public IEnumerator SingleAssetShouldBeLoadedAsync()
        {
            // Arrange.
            Object loadedAsset = null;
            var databaseAsset = new DatabaseAsset<ScriptableAsset>(ASSET_DIRECTORY_NAME, ASSET_NAME, ASSET_TYPE_EXTENSION);
            var loadingCallbacks = new AssetLoadingCallbacks<ScriptableAsset>(
                onLoaded: asset => loadedAsset = asset,
                onFailed: exception => throw exception);

            // Act.
            _stopwatch.Restart();
            databaseAsset.LoadAsync(loadingCallbacks);
            if (loadedAsset == null)
            {
                yield return null;
            }

            _stopwatch.Stop();

            // Assert.
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(databaseAsset.IsLoaded);

            // Debug.
            Debug.Log($"Loaded [{loadedAsset.name}] " +
                      $"from {nameof(AssetDatabase)} " +
                      $"in {_stopwatch.ElapsedMilliseconds} ms.\n" +
                      $"Size: {databaseAsset.Size.ToHumanReadableString()}.");
        }

        private sealed class ScriptableAsset : ScriptableObject { }
    }
}