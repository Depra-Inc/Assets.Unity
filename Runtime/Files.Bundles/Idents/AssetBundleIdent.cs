// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using Depra.Assets.Idents;
using Depra.Assets.Unity.Runtime.Extensions;
using JetBrains.Annotations;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Idents
{
    public sealed class AssetBundleIdent : IAssetIdent
    {
        private const string EXTENSION = ".assetbundle";

        public static AssetBundleIdent Empty => new(string.Empty);
        public static AssetBundleIdent Invalid => new(nameof(Invalid));

        private readonly FileInfo _fileSystemInfo;

        public AssetBundleIdent(string path)
        {
            _fileSystemInfo = new FileInfo(path);
            _fileSystemInfo.Directory.CreateIfNotExists();
            
            Name = string.IsNullOrEmpty(Extension)
                ? _fileSystemInfo.Name
                : _fileSystemInfo.Name.Replace(Extension, string.Empty);

            AbsolutePathWithoutExtension = AbsolutePath.Replace(EXTENSION, string.Empty);
        }

        public AssetBundleIdent(string name, string directory = null) : this(name, directory, EXTENSION) { }

        public AssetBundleIdent(string name, string directory, string extension = null) : this(
            Path.Combine(directory, name + extension)) { }

        [UsedImplicitly]
        public string Name { get; }

        [UsedImplicitly]
        public string Extension => EXTENSION;

        [UsedImplicitly]
        public string NameWithExtension => Name + Extension;

        [UsedImplicitly]
        public string AbsolutePath => _fileSystemInfo.FullName;

        [UsedImplicitly]
        public string AbsoluteDirectoryPath => _fileSystemInfo.DirectoryName;

        public string AbsolutePathWithoutExtension { get; }

        string IAssetIdent.Uri => AbsolutePath;

        string IAssetIdent.RelativeUri => Name;
    }
}