// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;

namespace Depra.Assets.Runtime.Files.Bundles.Exceptions
{
	internal sealed class AssetBundleNotLoaded : Exception
	{
		private const string MESSAGE_FORMAT = "Asset bundle at path [{0}] was not loaded.";

		public AssetBundleNotLoaded(string path) :
			base(string.Format(MESSAGE_FORMAT, path)) { }
	}
}