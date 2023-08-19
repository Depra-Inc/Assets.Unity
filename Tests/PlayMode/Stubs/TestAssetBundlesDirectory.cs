﻿// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using static Depra.Assets.Unity.Runtime.Common.Constants;
using static Depra.Assets.Unity.Runtime.Common.Paths;

namespace Depra.Assets.Unity.Tests.PlayMode.Stubs
{
	internal sealed class TestAssetBundlesDirectory
	{
		private const string TESTS_FOLDER = nameof(Tests);

		public readonly string AbsolutePath;
		public readonly string ProjectRelativePath;

		public TestAssetBundlesDirectory()
		{
			ProjectRelativePath = PACKAGES_FOLDER_NAME + SEPARATOR +
			                      FullModuleName + SEPARATOR +
			                      TESTS_FOLDER + SEPARATOR +
			                      nameof(PlayMode) + SEPARATOR +
			                      ASSETS_FOLDER_NAME + SEPARATOR +
			                      ASSET_BUNDLES_FOLDER_NAME;

			AbsolutePath = Path.GetFullPath(ProjectRelativePath);
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