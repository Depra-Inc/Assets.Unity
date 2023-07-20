// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using Depra.Assets.Unity.Tests.PlayMode.Utils;
using UnityEngine;
using static Depra.Assets.Unity.Runtime.Common.Constants;

namespace Depra.Assets.Unity.Tests.PlayMode.Stubs
{
    internal sealed class TestAssetBundlesDirectory
    {
        private const string TESTS_FOLDER = "Tests";
        private const string ASSETS_FOLDER = "Assets";

        public readonly string AbsolutePath;

        public TestAssetBundlesDirectory(Type sourceType)
        {
            var sourceDirectory = TypeUtility.GetSourceDirectoryName(sourceType);
            var assetBundlesRootDirectoryPath = FindRootTestsFolder(sourceDirectory);
            CreateIfDoesNotExist(assetBundlesRootDirectoryPath);
            AbsolutePath = Path.Combine(assetBundlesRootDirectoryPath, PlatformName());
            CreateIfDoesNotExist(AbsolutePath);
        }

        private static string PlatformName() => Application.platform switch
        {
            _ => "Windows"
        };

        private static void CreateIfDoesNotExist(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        private static string FindRootTestsFolder(string sourcePath)
        {
            var index = sourcePath.IndexOf(TESTS_FOLDER, StringComparison.Ordinal);
            if (index < 0)
            {
                throw new InvalidOperationException();
            }

            var parentFolderName = sourcePath[..(index + TESTS_FOLDER.Length)];
            parentFolderName = Path.Combine(parentFolderName, ASSETS_FOLDER, ASSET_BUNDLES_FOLDER_NAME);

            return parentFolderName;
        }
    }
}