// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Assets.Files.Resource
{
	internal sealed class PathDoesNotContainResourcesFolder : Exception
	{
		public PathDoesNotContainResourcesFolder(string path) : base(
			$"The specified path '{path}' does not contain the Resources folder!") { }
	}
}