// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using System.Runtime.CompilerServices;

namespace Depra.Assets.Runtime.Common
{
    internal static class RequiredFile
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ensure(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                throw new FileNotFoundException($"File {filePath} not found");
            }
        }
    }
}