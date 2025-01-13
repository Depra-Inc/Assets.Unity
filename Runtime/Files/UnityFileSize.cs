// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;

namespace Depra.Assets.Files
{
	public static class UnityFileSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FileSize FromProfiler(Object asset) => new(Profiler.GetRuntimeMemorySizeLong(asset));
	}
}