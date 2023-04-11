// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Tests.PlayMode.Mocks
{
    internal sealed class TestAssetBundlesDirectory
    {
        private const string TESTS_FOLDER = "Tests";
        private const string ASSETS_FOLDER = "Assets";

        public readonly string AbsolutePath;

        public TestAssetBundlesDirectory(Type sourceType)
        {
            var sourceDirectory = GetSourceDirectoryName(sourceType);
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
            var directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Exists == false)
            {
                directoryInfo.Create();
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
            parentFolderName = Path.Combine(parentFolderName, ASSETS_FOLDER, AssetBundlesFolderName);

            return parentFolderName;
        }

        private static string GetSourceDirectoryName(Type type)
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();
            if (frames == null)
            {
                throw new Exception($"Can't found directory for {type.Name}!");
            }

            foreach (var frame in frames)
            {
                if (frame.GetMethod() is { } method && method.DeclaringType == type)
                {
                    return Path.GetDirectoryName(frame.GetFileName());
                }
            }

            throw new Exception($"Can't found directory for {type.Name}!");
        }
    }
}