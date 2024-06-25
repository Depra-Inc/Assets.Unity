// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine.Networking;

namespace Depra.Asset.Files.Bundles.Extensions
{
	internal static class UnityWebRequestExtensions
	{
		public static bool CanGetResult(this UnityWebRequest self) =>
			self.result is UnityWebRequest.Result.ConnectionError
				or UnityWebRequest.Result.DataProcessingError
				or UnityWebRequest.Result.ProtocolError;

		public static int ParseSize(this UnityWebRequest self)
		{
			var contentLength = self.GetResponseHeader("Content-Length");
			if (int.TryParse(contentLength, out var returnValue))
			{
				return returnValue;
			}

			return -1;
		}
	}
}