// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Assets.ValueObjects;
using UnityEngine;
using UnityEngine.Profiling;

namespace Depra.Assets.Files
{
	public static class UnityFileSize
	{
		public static FileSize FromProfiler(Object asset) => new(Profiler.GetRuntimeMemorySizeLong(asset));
	}
}