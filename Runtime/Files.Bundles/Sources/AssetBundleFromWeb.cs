// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Extensions;
using Depra.Assets.ValueObjects;
using UnityEngine;
using UnityEngine.Networking;

namespace Depra.Assets.Runtime.Files.Bundles.Sources
{
	public sealed class AssetBundleFromWeb : IAssetBundleSource
	{
		private long _contentSize;

		FileSize IAssetBundleSource.Size(AssetBundle of) =>
			_contentSize == -1 ? of.Size() : new FileSize(_contentSize);

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

		async Task<AssetBundle> IAssetBundleSource.LoadAsync(string by, Action<float> with,
			CancellationToken cancellationToken)
		{
			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(by);
			await webRequest
				.SendWebRequest()
				.ToTask(with, cancellationToken);

			var downloadedBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
			_contentSize = webRequest.ParseSize();
			webRequest.Dispose();

			return downloadedBundle;
		}
	}
}