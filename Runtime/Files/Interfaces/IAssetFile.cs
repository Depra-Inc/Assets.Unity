// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using Depra.Assets.Runtime.Files.Structs;

namespace Depra.Assets.Runtime.Files.Interfaces
{
    public interface IAssetFile
    {
        string Name { get; }
        
        string Path { get; }
        
        FileSize Size { get; }
    }
}