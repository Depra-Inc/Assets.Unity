// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Asset.Files.Bundles.Exceptions
{
	internal sealed class AssetBundleNotLoaded : Exception
	{
		public AssetBundleNotLoaded(string path) : base($"Asset bundle at path '{path}' was not loaded!") { }
	}
}