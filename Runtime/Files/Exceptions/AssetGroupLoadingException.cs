// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;

namespace Depra.Assets.Runtime.Files.Exceptions
{
    internal sealed class AssetGroupLoadingException : Exception
    {
        private const string MESSAGE_FORMAT = "Failed to load an asset from a group {0}!";

        public AssetGroupLoadingException(string groupName) : base(string.Format(MESSAGE_FORMAT, groupName)) { }
    }
}