// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Assets.Runtime.Files.Bundles.Exceptions
{
	internal sealed class AssetBundleNotLoaded : Exception
	{
		private const string MESSAGE_FORMAT = "Asset bundle at path '{0}' was not loaded.";

		public AssetBundleNotLoaded(string path) :
			base(string.Format(MESSAGE_FORMAT, path)) { }
	}
}