// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.Asset.ValueObjects;
using UnityEngine;
using UnityEngine.Profiling;

namespace Depra.Asset.Files
{
	internal static class UnityFileSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FileSize FromProfiler(Object asset) => new(Profiler.GetRuntimeMemorySizeLong(asset));
	}
}