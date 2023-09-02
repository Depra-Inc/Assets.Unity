// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Sources
{
	public interface IAssetBundleSource
	{
		FileSize Size(AssetBundle of);

		AssetBundle Load(string by);

		Task<AssetBundle> LoadAsync(string by, Action<float> with,
			CancellationToken cancellationToken = default);
	}
}