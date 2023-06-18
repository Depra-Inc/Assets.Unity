// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.ValueObjects;

namespace Depra.Assets.Runtime.Files.Interfaces
{
    public interface IAssetFile
    {
        IAssetIdent Ident { get; }

        FileSize Size { get; }
    }
}