// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Asset.Files.Bundles.Extensions
{
	internal static class AssetBundleRequestExtensions
	{
		public static Task<Object> ToTask(this AssetBundleRequest self, Action<float> onProgress = null,
			CancellationToken cancellationToken = default) => cancellationToken.IsCancellationRequested
			? Task.FromCanceled<Object>(cancellationToken)
			: self.isDone
				? Task.FromResult(self.asset)
				: AwaitWithProgress(self, onProgress, cancellationToken);

		private async static Task<Object> AwaitWithProgress(this AssetBundleRequest self,
			Action<float> onProgress, CancellationToken cancellationToken = default)
		{
			while (self.isDone == false)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return await Task.FromCanceled<Object>(cancellationToken);
				}

				onProgress?.Invoke(self.progress);
				await Task.Yield();
			}

			return self.asset;
		}
	}
}