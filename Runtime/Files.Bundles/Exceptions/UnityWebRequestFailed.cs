// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEngine.Networking;

namespace Depra.Asset.Files.Bundles.Exceptions
{
	internal sealed class UnityWebRequestFailed : Exception
	{
		private readonly string _text;
		private readonly string _error;

		private string _message;

		public UnityWebRequestFailed(UnityWebRequest webRequest)
		{
			_error = webRequest.error;
			if (webRequest.downloadHandler is DownloadHandlerBuffer buffer)
			{
				_text = buffer.text;
			}

			webRequest.GetResponseHeaders();
		}

		public override string Message => _message ??= string.IsNullOrWhiteSpace(_text) == false
			? _error + Environment.NewLine + _text
			: _error;
	}
}