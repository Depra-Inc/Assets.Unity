// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Bundles.Extensions;
using Depra.Assets.ValueObjects;
using UnityEngine;
using UnityEngine.Networking;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Sources
{
	public sealed class AssetBundleFromWeb : IAssetBundleSource
	{
		private long _contentSize;

		FileSize IAssetBundleSource.Size(AssetBundle of) =>
			_contentSize == -1 ? of.Size() : new FileSize(_contentSize);

		AssetBundle IAssetBundleSource.Load(string by)
		{
			using var request = UnityWebRequestAssetBundle.GetAssetBundle(by);
			request.SendWebRequest();

			while (request.isDone == false)
			{
				// Spinning for Synchronous Behavior (blocking).
			}

			Guard.AgainstInvalidRequestResult(request,
				(error, url) => new RemoteAssetBundleNotLoadedException(url, error));

			return DownloadHandlerAssetBundle.GetContent(request);
		}

		async UniTask<AssetBundle> IAssetBundleSource.LoadAsync(string by, IProgress<float> with,
			CancellationToken cancellationToken)
		{
			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(by);
			await webRequest
				.SendWebRequest()
				.ToUniTask(with, cancellationToken: cancellationToken);

			Guard.AgainstInvalidRequestResult(webRequest,
				(error, url) => new RemoteAssetBundleNotLoadedException(url, error));

			var downloadedBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
			_contentSize = webRequest.ParseSize();
			webRequest.Dispose();

			return downloadedBundle;
		}

		private sealed class RemoteAssetBundleNotLoadedException : Exception
		{
			private const string MESSAGE_FORMAT = "Error request [{0}, {1}]";

			public RemoteAssetBundleNotLoadedException(string url, string error) :
				base(string.Format(MESSAGE_FORMAT, url, error)) { }
		}
	}

	internal static class UnityWebRequestExtensions
	{
		public static bool CanGetResult(this UnityWebRequest request) =>
			request.result != UnityWebRequest.Result.ProtocolError &&
			request.result != UnityWebRequest.Result.ConnectionError;

		public static int ParseSize(this UnityWebRequest request)
		{
			var contentLength = request.GetResponseHeader("Content-Length");
			if (int.TryParse(contentLength, out var returnValue))
			{
				return returnValue;
			}

			return -1;
		}
	}
}