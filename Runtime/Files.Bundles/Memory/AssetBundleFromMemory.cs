// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Idents;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Memory
{
    public sealed class AssetBundleFromMemory : AssetBundleFile
    {
        private AssetBundleCreateRequest _createRequest;

        public AssetBundleFromMemory(FileSystemAssetIdent ident) : base(ident) { }

        protected override AssetBundle LoadOverride()
        {
            RequiredFile.Ensure(Path);

            var bytes = ReadBytes();
            var loadedBundle = AssetBundle.LoadFromMemory(bytes);

            return loadedBundle;
        }

        protected override async UniTask<AssetBundle> LoadAsyncOverride(CancellationToken cancellationToken,
           IProgress<float> progress = null)
        {
            RequiredFile.Ensure(Path);

            _createRequest = AssetBundle.LoadFromMemoryAsync(ReadBytes());
            return await _createRequest.ToUniTask(progress, cancellationToken: cancellationToken);
        }

        private void CancelRequest()
        {
            if (_createRequest == null || _createRequest.assetBundle == null)
            {
                return;
            }

            _createRequest.assetBundle.Unload(true);
            _createRequest = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] ReadBytes() => File.ReadAllBytes(Path);
    }
}