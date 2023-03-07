using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Depra.Assets.Runtime.Common
{
    internal static class Static
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CombineIntoPath(string directory, string name) => 
            Path.Combine(directory, name);

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