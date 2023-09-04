// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Reflection;

namespace Depra.Assets.Runtime.Files.Database
{
	internal sealed class AssetCatNotBeCreated : Exception
	{
		private const string MESSAGE_FORMAT = "Asset {0} with type {1} can not be created!";

		public AssetCatNotBeCreated(MemberInfo assetType, string assetName) :
			base(string.Format(MESSAGE_FORMAT, assetName, assetType.Name)) { }
	}
}