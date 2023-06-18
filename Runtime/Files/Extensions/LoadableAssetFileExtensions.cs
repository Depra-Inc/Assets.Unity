// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using Depra.Assets.Runtime.Files.Interfaces;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Extensions
{
    public static class LoadableAssetFileExtensions
    {
        public static string ReadTextFromFile(this ILoadableAsset<TextAsset> file)
        {
            var textAsset = file.Load();
            var text = textAsset.text;
            file.Unload();

            return text;
        }
    }
}