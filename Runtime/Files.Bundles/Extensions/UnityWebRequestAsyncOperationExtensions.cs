// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Asset.Files.Bundles.Exceptions;
using UnityEngine.Networking;

namespace Depra.Asset.Files.Bundles.Extensions
{
	internal static class UnityWebRequestAsyncOperationExtensions
	{
		public static Task<UnityWebRequest> ToTask(this UnityWebRequestAsyncOperation self,
			Action<float> onProgress = null, CancellationToken cancellationToken = default)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<UnityWebRequest>(cancellationToken);
			}

			if (self.isDone)
			{
				return self.webRequest.CanGetResult()
					? Task.FromResult(self.webRequest)
					: Task.FromException<UnityWebRequest>(new UnityWebRequestFailed(self.webRequest));
			}

			return AwaitWithProgress(self, onProgress, cancellationToken);
		}

		private async static Task<UnityWebRequest> AwaitWithProgress(this UnityWebRequestAsyncOperation self,
			Action<float> onProgress, CancellationToken cancellationToken = default)
		{
			while (self.isDone == false)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return await Task.FromCanceled<UnityWebRequest>(cancellationToken);
				}

				onProgress?.Invoke(self.progress);
				await Task.Yield();
			}

			return self.webRequest;
		}
	}
}