// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Assets.Common
{
	public static class UnityProject
	{
		public const string ASSETS_FOLDER_NAME = "Assets";
		public const string PACKAGES_FOLDER_NAME = "Packages";
		public const string RESOURCES_FOLDER_NAME = "Resources";

		public const string ASSET_BUNDLES_FOLDER_NAME = "AssetBundles";
		public const string STREAMING_ASSETS_FOLDER_NAME = "StreamingAssets";

		internal const string SLASH = "/";

		internal static string DataPathByPlatform
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