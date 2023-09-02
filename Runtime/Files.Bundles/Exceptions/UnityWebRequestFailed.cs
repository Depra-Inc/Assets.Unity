using System;
using UnityEngine.Networking;

namespace Depra.Assets.Runtime.Files.Bundles.Exceptions
{
	internal sealed class UnityWebRequestFailed : Exception
	{
		private readonly string _text;
		private readonly string _error;

		private string _message;

		public UnityWebRequestFailed(UnityWebRequest unityWebRequest)
		{
			_error = unityWebRequest.error;
			if (unityWebRequest.downloadHandler is DownloadHandlerBuffer buffer)
			{
				_text = buffer.text;
			}

			unityWebRequest.GetResponseHeaders();
		}

		public override string Message => _message ??= string.IsNullOrWhiteSpace(_text) == false
			? _error + Environment.NewLine + _text
			: _error;
	}
}