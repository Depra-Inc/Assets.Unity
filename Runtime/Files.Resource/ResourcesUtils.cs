using System;
using System.IO;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Extensions;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Runtime.Files.Resource
{
    internal static class ResourcesUtils
    {
        public static string ToRelativeResourcesFilePath(this string absolutePath) =>
            FindRelativePath(absolutePath).RemoveExtension();

        public static string ToRelativeResourcesDirectoryPath(this string absolutePath) =>
            Path.GetDirectoryName(FindRelativePath(absolutePath));

        private static string FindRelativePath(string absolutePath)
        {
            var resourcesIndex = absolutePath.IndexOf(RESOURCES_FOLDER_NAME, StringComparison.Ordinal);
            Guard.AgainstEqual(resourcesIndex, -1, () => new PathDoesNotContainResourcesFolderException());
            var relativePath = absolutePath[(resourcesIndex + RESOURCES_FOLDER_NAME.Length + 1)..];

            return relativePath;
        }
    }
}