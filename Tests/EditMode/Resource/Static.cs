using System;
using System.Collections.Generic;
using System.IO;
using Depra.Assets.Runtime.Factory;
using Depra.Assets.Runtime.Resource.Files;
using Depra.Assets.Runtime.Resource.Loading;
using Depra.Assets.Tests.Common;

namespace Depra.Assets.Tests.EditMode.Resource
{
    internal static class Static
    {
        internal const string ASSET_DIRECTORY = "";
        internal const string ASSET_NAME = "TestAsset";
        internal static Type ASSET_TYPE = typeof(TestAsset);
        private const string RESOURCES_FOLDER_NAME = "Resources";
        
        public static TestAsset CreateTestAsset(AssetFactory assetFactory, string assetName)
        {
            var fullDirectory = Path.Combine(RESOURCES_FOLDER_NAME, ASSET_DIRECTORY);
            return assetFactory.CreateAsset<TestAsset>(fullDirectory, assetName);
        }

        public static IEnumerable<ResourceAssetSyncLoadingStrategy> ResourceLoaders()
        {
            yield return new LoadingAnyResourceAsset();
            yield return new LoadingResourceAssetByPath(ASSET_NAME);
        }
    }
}