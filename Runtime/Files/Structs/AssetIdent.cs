// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

namespace Depra.Assets.Runtime.Files.Structs
{
    public readonly struct AssetIdent
    {
        public readonly string Name;
        public readonly string Path;
        public readonly string Extension;

        public AssetIdent(string path)
        {
            Path = path;
            Extension = System.IO.Path.GetExtension(Path);
            Name = System.IO.Path.GetFileNameWithoutExtension(Path);
        }

        public AssetIdent(string name, string directory, string extension = null)
        {
            Name = name;
            Path = System.IO.Path.Combine(directory, Name);
            Extension = extension ?? System.IO.Path.GetExtension(Path);
        }
    }
}