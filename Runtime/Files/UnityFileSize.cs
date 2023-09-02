// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using Depra.Assets.ValueObjects;
using UnityEngine;
using UnityEngine.Profiling;

namespace Depra.Assets.Runtime.Files
{
	public static class UnityFileSize
	{
		public static FileSize FromProfiler(Object asset) => new(Profiler.GetRuntimeMemorySizeLong(asset));
	}
}