// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
using static Depra.Assets.Runtime.Common.Module;
using static Depra.Assets.Runtime.Common.Paths;

namespace Depra.Assets.Tests.PlayMode.Stubs
{
	internal sealed class TestAssetBundlesDirectory
	{
		private const string TESTS_FOLDER = nameof(Tests);

		public readonly string AbsolutePath;
		public readonly string ProjectRelativePath;

		public TestAssetBundlesDirectory()
		{
			ProjectRelativePath = PACKAGES_FOLDER_NAME + SLASH +
			                      FullModuleName + SLASH +
			                      TESTS_FOLDER + SLASH +
			                      nameof(PlayMode) + SLASH +
			                      ASSETS_FOLDER_NAME + SLASH +
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