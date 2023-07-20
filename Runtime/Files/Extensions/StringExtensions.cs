// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using static Depra.Assets.Unity.Runtime.Common.Paths;

namespace Depra.Assets.Unity.Runtime.Files.Extensions
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