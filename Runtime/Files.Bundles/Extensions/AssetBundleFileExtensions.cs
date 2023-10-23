// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using Depra.Assets.Files;
using UnityEngine;

namespace Depra.Assets.Files.Bundles.Extensions
{
	public static class AssetBundleFileExtensions
	{
		public static IEnumerable<string> AllAssetNames(this ILoadableAsset<AssetBundle> self) =>
			self.Load().GetAllAssetNames();
	}
}