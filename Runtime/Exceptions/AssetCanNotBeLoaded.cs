// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Assets
{
	internal sealed class AssetCanNotBeLoaded : Exception
	{
		public AssetCanNotBeLoaded(string message) : base(message) { }
	}
}