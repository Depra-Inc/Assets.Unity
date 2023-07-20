// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Depra.Assets.Unity.Runtime.Extensions;
using UnityEditor;
using UnityEngine;
using static Depra.Assets.Unity.Runtime.Common.Paths;

namespace Depra.Assets.Unity.Tests.PlayMode.Utils
{
    internal static class PrepareEnvironment
    {
        private const string TESTS_FOLDER_NAME = "Tests";

        private const BuildAssetBundleOptions
            OPTIONS = BuildAssetBundleOptions.None | BuildAssetBundleOptions.StrictMode;

        private static readonly Dictionary<BuildTarget, string> PLATFORM_NAMES = new()
        {
            { BuildTarget.NoTarget, "Base" },
            { BuildTarget.Android, BuildTarget.Android.ToString() },
            { BuildTarget.iOS, BuildTarget.iOS.ToString() },
            { BuildTarget.StandaloneLinux64, "Linux" },
            { BuildTarget.StandaloneOSX, "MacOS" },
            { BuildTarget.StandaloneWindows64, "Windows" },
            { BuildTarget.StandaloneWindows, "Windows" },
        };

        public static void BuildAssetBundle(string targetPath, BuildTarget platform, string localPath) { }

        public static AssetBundle AssetBundle(string name, string absoluteDirectoryPath)
        {
            var assetBundlePath = Path.Combine(absoluteDirectoryPath);

            // Create the array of bundle build details.
            var bundlesForBuild = new AssetBundleBuild[]
            {
                new()
                {
                    assetBundleName = name,
                    assetNames = new[] { "TestAsset" }
                }
            };

            var manifest = BuildPipeline.BuildAssetBundles(assetBundlePath, bundlesForBuild, OPTIONS,
                EditorUserBuildSettings.activeBuildTarget);

            var assetBundle = UnityEngine.AssetBundle.LoadFromFile(assetBundlePath);

            return assetBundle;
        }

        public static DirectoryInfo AssetBundleDirectory(Type sourceType)
        {
            var sourceDirectory = TypeUtility.GetSourceDirectoryName(sourceType);
            var assetBundlesRootDirectory = FindRootTestsFolder(sourceDirectory);
            assetBundlesRootDirectory.CreateIfNotExists();

            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            var platformName = PLATFORM_NAMES[activeBuildTarget];
            var assetBundlesByPlatformDirectoryPath = Path.Combine(assetBundlesRootDirectory.FullName, platformName);
            var assetBundlesByPlatformDirectory = new DirectoryInfo(assetBundlesByPlatformDirectoryPath);
            assetBundlesByPlatformDirectory.CreateIfNotExists();

            return assetBundlesByPlatformDirectory;
        }

        private static AssetBundleManifest CreateManifest(string outputDirectory, BuildTarget platform)
        {
            var bundlesForBuild = FindBundlesForBuild(outputDirectory);
            var manifest = BuildPipeline.BuildAssetBundles(outputDirectory, bundlesForBuild, OPTIONS, platform);
            if (manifest == null)
            {
                throw new ApplicationException($"Failed to build {nameof(AssetBundleManifest)}");
            }

            return manifest;
        }

        private static AssetBundleBuild[] FindBundlesForBuild(string rootDirectory) =>
            AssetDatabase.GetAllAssetBundleNames()
                .Select(bundle =>
                {
                    var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundle)
                        .Where(asset => asset.StartsWith(rootDirectory, StringComparison.InvariantCulture))
                        .ToArray();

                    return (bundle, assetPaths);
                })
                .Where(group => group.assetPaths is { Length: > 0 })
                .Select(group => new AssetBundleBuild()
                {
                    assetNames = group.assetPaths,
                    assetBundleName = group.bundle,
                })
                .ToArray();

        private static DirectoryInfo FindRootTestsFolder(string sourcePath)
        {
            var index = sourcePath.IndexOf(TESTS_FOLDER_NAME, StringComparison.Ordinal);
            if (index < 0)
            {
                throw new InvalidOperationException();
            }

            var testsDirectoryPath = sourcePath[..(index + TESTS_FOLDER_NAME.Length)];
            testsDirectoryPath = Path.Combine(testsDirectoryPath, ASSETS_FOLDER_NAME, ASSET_BUNDLES_FOLDER_NAME);

            return new DirectoryInfo(testsDirectoryPath);
        }
    }
}