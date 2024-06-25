// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Asset.Files.Bundles.Exceptions;
using Depra.Asset.Files.Bundles.Extensions;
using Depra.Asset.ValueObjects;
using UnityEngine;
using UnityEngine.Networking;

namespace Depra.Asset.Files.Bundles.Sources
{
	public sealed class AssetBundleFromWeb : IAssetBundleSource
	{
		private long _contentSize;

		FileSize IAssetBundleSource.Size(AssetBundle of) =>
			_contentSize == -1 ? AssetBundleSize.Evaluate(of) : new FileSize(_contentSize);

		AssetBundle IAssetBundleSource.Load(string by)
		{
			using var unityWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(by);
			unityWebRequest.SendWebRequest();

			while (unityWebRequest.isDone == false)
			{
				// Spinning for Synchronous Behavior (blocking).
			}

			if (unityWebRequest.CanGetResult() == false)
			{
				throw new UnityWebRequestFailed(unityWebRequest);
			}

			return DownloadHandlerAssetBundle.GetContent(unityWebRequest);
		}

		async Task<AssetBundle> IAssetBundleSource.LoadAsync(string by, Action<float> onProgress,
			CancellationToken cancellationToken)
		{
			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(by);
			await webRequest
				.SendWebRequest()
				.ToTask(onProgress, cancellationToken);

			var downloadedBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
			_contentSize = webRequest.ParseSize();
			webRequest.Dispose();

			return downloadedBundle;
		}
	}
}