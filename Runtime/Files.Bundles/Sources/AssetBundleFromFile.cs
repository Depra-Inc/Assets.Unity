// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Extensions;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Sources
{
	public readonly struct AssetBundleFromFile : IAssetBundleSource
	{
		FileSize IAssetBundleSource.Size(AssetBundle of) => of.Size();

		AssetBundle IAssetBundleSource.Load(string by)
		{
			Guard.AgainstFileNotFound(by);
			var loadedBundle = AssetBundle.LoadFromFile(by);

			return loadedBundle;
		}

		Task<AssetBundle> IAssetBundleSource.LoadAsync(string by, Action<float> with,
			CancellationToken cancellationToken)
		{
			Guard.AgainstFileNotFound(by);

			return AssetBundle
				.LoadFromFileAsync(by)
				.ToTask(with, cancellationToken);
		}
	}
}