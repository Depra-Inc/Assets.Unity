﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Bundles.Extensions;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Sources
{
    public readonly struct AssetBundleFromMemory : IAssetBundleSource
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] ReadBytes(string path) => File.ReadAllBytes(path);

        FileSize IAssetBundleSource.Size(AssetBundle of) => of.Size();

        AssetBundle IAssetBundleSource.Load(string by)
        {
            Guard.AgainstFileNotFound(by);

            var bytes = ReadBytes(by);
            var loadedBundle = AssetBundle.LoadFromMemory(bytes);

            return loadedBundle;
        }

        async UniTask<AssetBundle> IAssetBundleSource.LoadAsync(string by, IProgress<float> with,
            CancellationToken cancellationToken)
        {
            Guard.AgainstFileNotFound(by);

            var asyncRequest = AssetBundle
                .LoadFromMemoryAsync(ReadBytes(by))
                .ToUniTask(with, cancellationToken: cancellationToken);

            return await asyncRequest;
        }
    }
}