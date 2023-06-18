// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Linq;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Extensions
{
    public static class LoadableAssetFileExtensions
    {
        public static FileSize SizeForAll<TAsset>(this IEnumerable<ILoadableAsset<TAsset>> assets) =>
            new(assets.Sum(x => x.Size.SizeInBytes));

        public static string ReadTextFromFile(this ILoadableAsset<TextAsset> file)
        {
            var textAsset = file.Load();
            var text = textAsset.text;
            file.Unload();

            return text;
        }
    }
}