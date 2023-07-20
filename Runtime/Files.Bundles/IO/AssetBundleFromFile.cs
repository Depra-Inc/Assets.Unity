// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Bundles.Files;
using Depra.Assets.Unity.Runtime.Files.Idents;
using UnityEngine;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.IO
{
    public sealed class AssetBundleFromFile : AssetBundleFile
    {
        public AssetBundleFromFile(FileSystemAssetIdent ident) : base(ident) { }

        protected override AssetBundle LoadOverride()
        {
            Guard.AgainstFileNotFound(Ident.Uri);
            return AssetBundle.LoadFromFile(Ident.Uri);
        }

        protected override async UniTask<AssetBundle> LoadAsyncOverride(IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            Guard.AgainstFileNotFound(Ident.Uri);
            var createRequest = AssetBundle.LoadFromFileAsync(Ident.Uri);
            return await createRequest.ToUniTask(progress, cancellationToken: cancellationToken);
        }
    }
}