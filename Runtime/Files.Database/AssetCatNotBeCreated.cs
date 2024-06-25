// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Reflection;

namespace Depra.Asset.Files.Database
{
	internal sealed class AssetCatNotBeCreated : Exception
	{
		public AssetCatNotBeCreated(MemberInfo assetType, string assetName) : base(
			$"Asset '{assetName}' with type '{assetType.Name}' can not be created!") { }
	}
}