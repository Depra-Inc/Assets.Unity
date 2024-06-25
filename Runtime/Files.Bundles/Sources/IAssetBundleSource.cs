// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Asset.ValueObjects;
using UnityEngine;

namespace Depra.Asset.Files.Bundles.Sources
{
	public interface IAssetBundleSource
	{
		FileSize Size(AssetBundle of);

		AssetBundle Load(string by);

		Task<AssetBundle> LoadAsync(string by, Action<float> onProgress, CancellationToken cancellationToken = default);
	}
}