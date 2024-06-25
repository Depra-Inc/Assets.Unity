// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;

namespace Depra.Asset.Extensions
{
	public static class StringExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToUnixPath(this string path) => path.Replace('\\', '/');
	}
}