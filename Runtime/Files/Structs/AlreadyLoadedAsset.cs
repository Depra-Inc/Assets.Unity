﻿// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using Depra.Assets.Runtime.Async.Tokens;

namespace Depra.Assets.Runtime.Files.Structs
{
    public readonly struct AlreadyLoadedAsset<TAsset>
    {
        public static IAsyncToken Create(TAsset loadedAsset, Action<TAsset> onLoaded,
            Action<DownloadProgress> onProgress)
        {
            onProgress?.Invoke(DownloadProgress.Full);
            onLoaded.Invoke(loadedAsset);

            return AsyncActionToken.Empty;
        }
    }
}