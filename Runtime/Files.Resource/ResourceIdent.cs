// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using Depra.Assets.Runtime.Extensions;
using Depra.Assets.Runtime.Files.Extensions;
using Depra.Assets.Runtime.Files.Idents;
using JetBrains.Annotations;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Runtime.Files.Resource
{
    public sealed class ResourceIdent : IAssetIdent
    {
        private readonly FileInfo _fileSystemInfo;

        public static ResourceIdent Empty => new(string.Empty, string.Empty);
        public static ResourceIdent Invalid => new(string.Empty, nameof(Invalid));

        public ResourceIdent(string absolutePath)
        {
            _fileSystemInfo = new FileInfo(absolutePath);
            _fileSystemInfo.Directory.CreateIfNotExists();

            Name = _fileSystemInfo.Name.RemoveExtension();
            RelativePath = AbsolutePath.ToRelativeResourcesFilePath();
            RelativeProjectPath = AbsolutePath.ToRelativeUnityPath();
            RelativeDirectoryPath = AbsolutePath.ToRelativeResourcesDirectoryPath();
        }

        public ResourceIdent(string relativeDirectory, string name, string extension = null)
        {
            Name = name;
            RelativeDirectoryPath = relativeDirectory;
            var absolutePath = Path.GetFullPath(Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME,
                relativeDirectory, name + extension));
            _fileSystemInfo = new FileInfo(absolutePath);
            RelativePath = AbsolutePath.ToRelativeResourcesFilePath();
            RelativeProjectPath = AbsolutePath.ToRelativeUnityPath();
            RelativeDirectoryPath = AbsolutePath.ToRelativeResourcesDirectoryPath();
        }

        public string Name { get; }
        
        [UsedImplicitly]
        public string Extension => _fileSystemInfo.Extension;

        [UsedImplicitly]
        public string NameWithExtension => Name + Extension;

        public string RelativePath { get; }
        public string AbsolutePath => _fileSystemInfo.FullName;

        public string RelativeProjectPath { get; }

        [UsedImplicitly]
        public string RelativeDirectoryPath { get; }

        public string AbsoluteDirectoryPath => _fileSystemInfo.DirectoryName;

        string IAssetIdent.Uri => AbsolutePath;
        string IAssetIdent.RelativeUri => RelativePath;
    }
}