// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
using System.Runtime.CompilerServices;

namespace Depra.Assets
{
	public static class DirectoryInfoExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DirectoryInfo Require(this DirectoryInfo self)
		{
			if (self.Exists == false)
			{
				self.Create();
			}

			return self;
		}
	}
}