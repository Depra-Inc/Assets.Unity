// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.IO;
using Depra.Assets.Idents;
using UnityEngine;
using static Depra.Assets.Unity.Runtime.Common.Paths;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Idents
{
    public sealed class AssetBundleIdent : IAssetIdent
    {
        private const string EXTENSION = ".assetbundle";

        public AssetBundleIdent(string name, string directory = null)
        {
            Name = name;
            AbsoluteDirectoryPath = directory ?? Application.streamingAssetsPath;
            AbsolutePath = Path.Combine(AbsoluteDirectoryPath, Name + Extension);
            RelativePath = Path.GetRelativePath(DataPathByPlatform, AbsolutePath);
        }

        public string Name { get; }
        public string Extension => EXTENSION;

        public string RelativePath { get; }
        public string AbsolutePath { get; }

        public string AbsoluteDirectoryPath { get; }

        string IAssetIdent.Uri => AbsolutePath;
        string IAssetIdent.RelativeUri => RelativePath;
    }
}