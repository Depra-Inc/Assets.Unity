// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using static Depra.Assets.Unity.Runtime.Common.Constants;
using static Depra.Assets.Unity.Runtime.Common.Paths;

namespace Depra.Assets.Unity.Tests.PlayMode.Stubs
{
    internal sealed class TestAssetBundlesDirectory
    {
        private const string TESTS_FOLDER = "Tests";

        public readonly string AbsolutePath;

        public TestAssetBundlesDirectory()
        {
            var relativePath = PACKAGES_FOLDER_NAME + "/" +
                                    FullModuleName + "/" +
                                    TESTS_FOLDER + "/" +
                                    nameof(PlayMode) + "/" +
                                    ASSETS_FOLDER_NAME + "/" +
                                    ASSET_BUNDLES_FOLDER_NAME;

            AbsolutePath = Path.GetFullPath(relativePath);
            CreateIfDoesNotExist(AbsolutePath);
        }

        private static void CreateIfDoesNotExist(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}