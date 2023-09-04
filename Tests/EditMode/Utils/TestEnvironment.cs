// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor;
using UnityEngine;

namespace Depra.Assets.Tests.EditMode.Utils
{
	internal static class TestEnvironment
	{
		public static TAsset CreateAsset<TAsset>(string path) where TAsset : ScriptableObject
		{
			var asset = ScriptableObject.CreateInstance<TAsset>();
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			return asset;
		}

		public static bool TryDeleteAsset(Object asset)
		{
			if (asset == null)
			{
				return false;
			}

			var assetPath = AssetDatabase.GetAssetPath(asset);
			if (assetPath == null)
			{
				return false;
			}

			AssetDatabase.DeleteAsset(assetPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			return true;
		}
	}
}