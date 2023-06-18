// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

namespace Depra.Assets.Runtime.Files.Idents
{
    public sealed class NamedAssetIdent : IAssetIdent
    {
        public static NamedAssetIdent Empty => new(string.Empty);
        public static NamedAssetIdent Invalid => new(nameof(Invalid));

        public NamedAssetIdent(string uri)
        {
            Uri = uri;
            Name = uri;
        }

        public string Uri { get; }
        
        public string Name { get; }

        string IAssetIdent.RelativeUri => Uri;
    }
}