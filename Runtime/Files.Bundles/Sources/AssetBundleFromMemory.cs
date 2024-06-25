// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Depra.Asset.Exceptions;
using Depra.Asset.Files.Bundles.Extensions;
using Depra.Asset.ValueObjects;
using UnityEngine;

namespace Depra.Asset.Files.Bundles.Sources
{
	public readonly struct AssetBundleFromMemory : IAssetBundleSource
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte[] ReadBytes(string path) => File.ReadAllBytes(path);

		FileSize IAssetBundleSource.Size(AssetBundle of) => AssetBundleSize.Evaluate(of);

		AssetBundle IAssetBundleSource.Load(string by)
		{
			Guard.AgainstFileNotFound(by);

			return AssetBundle.LoadFromMemory(ReadBytes(by));
		}

		Task<AssetBundle> IAssetBundleSource.LoadAsync(string by, Action<float> onProgress, CancellationToken cancellationToken)
		{
			Guard.AgainstFileNotFound(by);

			return AssetBundle
				.LoadFromMemoryAsync(ReadBytes(by))
				.ToTask(onProgress, cancellationToken);
		}
	}
}