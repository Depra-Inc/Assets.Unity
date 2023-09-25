// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Common;

namespace Depra.Assets.Runtime.Extensions
{
	public static class DirectoryInfoExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmpty(this DirectoryInfo self) =>
			self.EnumerateFileSystemInfos().Any() == false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DeleteIfEmpty(this DirectoryInfo self)
		{
			if (self.Exists == false || self.IsEmpty() == false)
			{
				return;
			}

			self.Delete(true);
			File.Delete(self.FullName + AssetTypes.META);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DirectoryInfo CreateIfNotExists(this DirectoryInfo self)
		{
			if (self.Exists == false)
			{
				self.Create();
			}

			return self;
		}
	}
}