// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Depra.Assets.Extensions
{
	public static class DirectoryInfoExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmpty(this DirectoryInfo self) => self.EnumerateFileSystemInfos().Any() == false;

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