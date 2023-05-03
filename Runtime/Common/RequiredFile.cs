// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Depra.Assets.Runtime.Common
{
    internal static class RequiredFile
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ensure(string filePath, Action<Exception> onFailed)
        {
            if (File.Exists(filePath) == false)
            {
                onFailed?.Invoke(new FileNotFoundException($"File {filePath} not found"));
            }
        }
    }
}