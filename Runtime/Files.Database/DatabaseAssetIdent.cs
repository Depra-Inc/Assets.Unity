﻿using System.IO;
using Depra.Assets.Idents;
using JetBrains.Annotations;

namespace Depra.Assets.Unity.Runtime.Files.Database
{
    public readonly struct DatabaseAssetIdent : IAssetIdent
    {
        public static DatabaseAssetIdent Empty = new();

        public DatabaseAssetIdent(string relativeDirectory, string name, string extension)
        {
            Name = name;
            Extension = extension;
            NameWithExtension = Name + Extension;

            RelativePath = Path.Combine(relativeDirectory, NameWithExtension);

            AbsolutePath = Path.GetFullPath(RelativePath);
            AbsoluteDirectoryPath = Path.GetFullPath(relativeDirectory);
            AbsoluteDirectory = new DirectoryInfo(AbsoluteDirectoryPath);
        }

        public string Name { get; }

        [UsedImplicitly]
        public string Extension { get; }

        [UsedImplicitly]
        public string NameWithExtension { get; }

        public string RelativePath { get; }
        public string AbsolutePath { get; }

        [UsedImplicitly]
        public string AbsoluteDirectoryPath { get; }

        internal DirectoryInfo AbsoluteDirectory { get; }

        string IAssetIdent.Uri => AbsolutePath;
        string IAssetIdent.RelativeUri => RelativePath;
    }
}