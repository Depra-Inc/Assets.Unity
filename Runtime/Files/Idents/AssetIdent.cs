// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

namespace Depra.Assets.Runtime.Files.Idents
{
    public sealed class AssetIdent : IAssetIdent
    {
        public static AssetIdent Empty => new(string.Empty);
        public static AssetIdent Invalid => new(nameof(Invalid));

        public AssetIdent(string uri)
        {
            Uri = uri;
            Name = uri;
        }

        public string Name { get; }
        public string Uri { get; }
    }
}