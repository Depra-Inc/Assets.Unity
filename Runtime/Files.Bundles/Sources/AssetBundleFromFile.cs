// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Bundles.Extensions;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Sources
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

		UniTask<AssetBundle> IAssetBundleSource.LoadAsync(string by, IProgress<float> with,
			CancellationToken cancellationToken)
		{
			Guard.AgainstFileNotFound(by);

			var asyncRequest = AssetBundle
				.LoadFromFileAsync(by)
				.ToUniTask(with, cancellationToken: cancellationToken);

			return asyncRequest;
		}
	}
}