using System.IO;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Runtime.Files.Extensions
{
    internal static class StringExtensions
    {
        public static string RemoveExtension(this string path) =>
            Path.ChangeExtension(path, null);

        public static string ToRelativeUnityPath(this string absolutePath)
        {
            var dataPath = Path.GetFullPath(DataPathByPlatform);
            var relativeUnitPath = absolutePath.Replace(dataPath, ASSETS_FOLDER_NAME);

            return relativeUnitPath;
        }
    }
}