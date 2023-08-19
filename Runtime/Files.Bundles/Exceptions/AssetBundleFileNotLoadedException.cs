// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Exceptions
{
	internal sealed class AssetBundleFileNotLoadedException : Exception
	{
		private const string MESSAGE_FORMAT = "File with name [{0}] form asset bundle [{1}] was not loaded.";

		public AssetBundleFileNotLoadedException(string name, string bundleName) :
			base(string.Format(MESSAGE_FORMAT, name, bundleName)) { }
	}
}