using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Depra.Assets.Runtime.Files.Bundles.Web
{
    public static class AssetBundlesHelper
    {
        public static IEnumerator GetContentLength(string url, Action<int> response)
        {
            var request = UnityWebRequest.Head(url);
            yield return request.SendWebRequest();
            if (request.CanGetResult())
            {
                var contentLength = request.GetResponseHeader("Content-Length");

                if (int.TryParse(contentLength, out var returnValue))
                {
                    response(returnValue);
                }
                else
                {
                    response(-1);
                }
            }
            else
            {
                Debug.LogErrorFormat("Error request [{0}, {1}]", url, request.error);

                response(-1);
            }
        }
    }
}