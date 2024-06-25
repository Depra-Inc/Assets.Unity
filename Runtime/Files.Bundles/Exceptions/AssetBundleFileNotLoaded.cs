// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Asset.Files.Bundles.Exceptions
{
	internal sealed class AssetBundleFileNotLoaded : Exception
	{
		public AssetBundleFileNotLoaded(string name, string bundleName) : base(
			$"File with name '{name}' form asset bundle '{bundleName}' was not loaded!") { }
	}
}