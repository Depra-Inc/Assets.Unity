// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Common;
using Depra.Assets.Runtime.Files.Bundles.Files;
using Depra.Assets.Runtime.Files.Idents;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.IO
{
    public sealed class AssetBundleFromFile : AssetBundleFile
    {
        private AssetBundleCreateRequest _createRequest;

        public AssetBundleFromFile(FileSystemAssetIdent ident) : base(ident) { }

        protected override AssetBundle LoadOverride()
        {
            RequiredFile.Ensure(Ident.Uri);
            return AssetBundle.LoadFromFile(Ident.Uri);
        }

        protected override async UniTask<AssetBundle> LoadAsyncOverride(CancellationToken cancellationToken,
            IProgress<float> progress = null)
        {
            RequiredFile.Ensure(Ident.Uri);
            
            _createRequest = AssetBundle.LoadFromFileAsync(Ident.Uri);
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
    }
}