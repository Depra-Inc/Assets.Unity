// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Utils
{
    internal static class AssetDatabaseHelper
    {
        public static TAsset CreateAsset<TAsset>(string name, string directoryPath) where TAsset : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<TAsset>();
            asset = (TAsset)ActivateAsset(asset, name, directoryPath);

            return asset;
        }

        public static void DeleteAsset(Object asset)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static Object ActivateAsset(Object asset, string name, string directoryPath)
        {
            asset.name = name;
            var fullPath = Path.Combine(directoryPath, name);
            AssetDatabase.CreateAsset(asset, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }
    }
}