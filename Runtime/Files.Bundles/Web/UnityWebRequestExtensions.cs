// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using UnityEngine.Networking;

namespace Depra.Assets.Runtime.Files.Bundles.Web
{
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