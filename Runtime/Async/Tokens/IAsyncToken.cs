// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

namespace Depra.Assets.Runtime.Async.Tokens
{
    public interface IAsyncToken
    {
        bool IsCanceled { get; }

        void Cancel();
    }
}