// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Assets.Files.Resource
{
	internal sealed class ResourceNotLoaded : Exception
	{
		public ResourceNotLoaded(string assetPath) : base($"Resource was not loaded by path: {assetPath}!") { }
	}
}