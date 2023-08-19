// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using Depra.Assets.Files;
using UnityEngine;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Extensions
{
	public static class AssetBundleFileExtensions
	{
		public static IEnumerable<string> AllAssetNames(this ILoadableAsset<AssetBundle> self) =>
			self.Load().GetAllAssetNames();
	}
}