// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using UnityEngine;

namespace Depra.Assets.Unity.Runtime.Common
{
    internal static class Constants
    {
        public const string FRAMEWORK_NAME = "Depra";
        public const string MODULE_NAME = "Assets.Unity";
        public static readonly string FullModuleName = string.Join('.', FRAMEWORK_NAME, MODULE_NAME);

        public const string ASSETS_FOLDER_NAME = "Assets";
        public const string RESOURCES_FOLDER_NAME = "Resources";

        public const string ASSET_BUNDLES_FOLDER_NAME = "AssetBundles";
        public const string STREAMING_ASSETS_FOLDER_NAME = "StreamingAssets";
        
        public static string DataPathByPlatform
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath;
#else
                return Application.persistentDataPath;
#endif
            }
        }
    }
}