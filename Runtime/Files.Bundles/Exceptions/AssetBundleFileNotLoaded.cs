// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Assets.Files.Bundles.Exceptions
{
	internal sealed class AssetBundleFileNotLoaded : Exception
	{
		private const string MESSAGE_FORMAT = "File with name '{0}' form asset bundle '{1}' was not loaded!";

		public AssetBundleFileNotLoaded(string name, string bundleName) :
			base(string.Format(MESSAGE_FORMAT, name, bundleName)) { }
	}
}