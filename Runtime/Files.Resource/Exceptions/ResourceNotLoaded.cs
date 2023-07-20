// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;

namespace Depra.Assets.Unity.Runtime.Files.Resource.Exceptions
{
    internal sealed class ResourceNotLoaded : Exception
    {
        private const string MESSAGE_FORMAT = "Resource was not loaded by path: {0}!";

        public ResourceNotLoaded(string assetPath) :
            base(string.Format(MESSAGE_FORMAT, assetPath)) { }
    }
}