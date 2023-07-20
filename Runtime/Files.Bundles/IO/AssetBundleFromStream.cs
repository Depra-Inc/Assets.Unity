// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Bundles.Files;
using Depra.Assets.Unity.Runtime.Files.Idents;
using UnityEngine;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.IO
{
    public sealed class AssetBundleFromStream : AssetBundleFile
    {
        public AssetBundleFromStream(FileSystemAssetIdent ident) : base(ident) { }

        protected override AssetBundle LoadOverride()
        {
            Guard.AgainstFileNotFound(Ident.Uri);

            using var fileStream = OpenStream();
            var loadedBundle = AssetBundle.LoadFromStream(fileStream);

            return loadedBundle;
        }

        protected override async UniTask<AssetBundle> LoadAsyncOverride(IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            Guard.AgainstFileNotFound(Ident.Uri);

            await using var stream = OpenStream();
            var asyncRequest = AssetBundle
                .LoadFromStreamAsync(stream)
                .ToUniTask(progress, cancellationToken: cancellationToken);

            return await asyncRequest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stream OpenStream() => new FileStream(Ident.Uri, FileMode.Open, FileAccess.Read);
    }
}