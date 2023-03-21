using System;
using System.Text.RegularExpressions;

namespace Depra.Assets.Runtime.Common
{
    internal static class Static
    {
        public static bool BundlePathIsUrl(string path)
        {
            const string pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex.IsMatch(path);
            
            return Uri.TryCreate(path, UriKind.Absolute, out var uriResult) &&
                uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
        }
    }
}