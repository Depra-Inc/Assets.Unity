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

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Memory
{
    public sealed class AssetBundleFromMemory : AssetBundleFile
    {
        public AssetBundleFromMemory(FileSystemAssetIdent ident) : base(ident) { }

        protected override AssetBundle LoadOverride()
        {
            Guard.AgainstFileNotFound(Ident.Uri);

            var bytes = ReadBytes();
            var loadedBundle = AssetBundle.LoadFromMemory(bytes);

            return loadedBundle;
        }

        protected override async UniTask<AssetBundle> LoadAsyncOverride(IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            Guard.AgainstFileNotFound(Ident.Uri);

            var asyncRequest = AssetBundle
                .LoadFromMemoryAsync(ReadBytes())
                .ToUniTask(progress, cancellationToken: cancellationToken);

            return await asyncRequest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] ReadBytes() => File.ReadAllBytes(Ident.Uri);
    }
}