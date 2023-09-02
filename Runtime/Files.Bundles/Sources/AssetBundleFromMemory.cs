// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Extensions;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Sources
{
	public readonly struct AssetBundleFromMemory : IAssetBundleSource
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte[] ReadBytes(string path) => File.ReadAllBytes(path);

		FileSize IAssetBundleSource.Size(AssetBundle of) => of.Size();

		AssetBundle IAssetBundleSource.Load(string by)
		{
			Guard.AgainstFileNotFound(by);

			var bytes = ReadBytes(by);
			var loadedBundle = AssetBundle.LoadFromMemory(bytes);

			return loadedBundle;
		}

		async Task<AssetBundle> IAssetBundleSource.LoadAsync(string by, Action<float> with,
			CancellationToken cancellationToken)
		{
			Guard.AgainstFileNotFound(by);

			return await AssetBundle
				.LoadFromMemoryAsync(ReadBytes(by))
				.ToTask(with, cancellationToken);
		}
	}
}