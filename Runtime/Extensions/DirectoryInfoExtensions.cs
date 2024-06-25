// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
using System.Runtime.CompilerServices;

namespace Depra.Asset.Extensions
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