// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Depra.Asset.Files.Bundles.Extensions
{
	internal static class AssetBundleCreateRequestExtensions
	{
		public static Task<AssetBundle> ToTask(this AssetBundleCreateRequest self,
			Action<float> onProgress = null, CancellationToken cancellationToken = default)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<AssetBundle>(cancellationToken);
			}

			return self.isDone
				? Task.FromResult(self.assetBundle)
				: AwaitWithProgress(self, onProgress, cancellationToken);
		}

		private async static Task<AssetBundle> AwaitWithProgress(this AssetBundleCreateRequest self,
			Action<float> onProgress, CancellationToken cancellationToken = default)
		{
			while (self.isDone == false)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return await Task.FromCanceled<AssetBundle>(cancellationToken);
				}

				onProgress?.Invoke(self.progress);
				await Task.Yield();
			}

			return self.assetBundle;
		}
	}
}