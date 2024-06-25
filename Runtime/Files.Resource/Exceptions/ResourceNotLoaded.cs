// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Asset.Files.Resource.Exceptions
{
	internal sealed class ResourceNotLoaded : Exception
	{
		public ResourceNotLoaded(string assetPath) : base($"Resource was not loaded by path: {assetPath}!") { }
	}
}