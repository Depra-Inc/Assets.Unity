// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Depra.Assets.Common;
using Depra.Assets.ValueObjects;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Tests.PlayMode.Utils
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

		public static void CleanupDirectory(DirectoryInfo self)
		{
			if (self.Exists && IsEmpty(self))
			{
				DeleteDirectory(self);
			}
		}

		private static void DeleteDirectory(DirectoryInfo self)
		{
			self.Delete(true);
			File.Delete(self.FullName + AssetTypes.META);
		}

		private static bool IsEmpty(DirectoryInfo self) => self.EnumerateFileSystemInfos().Any() == false;
	}

	internal static class AssetUriExtensions
	{
		public static string Flatten(this IEnumerable<IAssetUri> assets) => assets.Aggregate(string.Empty,
			(current, asset) => current + (Environment.NewLine + asset.Absolute));
	}
}