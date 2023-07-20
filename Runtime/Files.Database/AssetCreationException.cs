// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Reflection;

namespace Depra.Assets.Unity.Runtime.Files.Database
{
    public sealed class AssetCreationException : Exception
    {
        private const string MESSAGE_FORMAT = "Asset {0} with type {1} can not be created!";

        public AssetCreationException(string message) : base(message) { }

        public AssetCreationException(MemberInfo assetType, string assetName) :
            base(string.Format(MESSAGE_FORMAT, assetName, assetType.Name)) { }
    }
}