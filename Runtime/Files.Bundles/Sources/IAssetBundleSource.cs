﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Unity.Runtime.Files.Bundles.Sources
{
    public interface IAssetBundleSource
    {
        FileSize Size(AssetBundle of);
        
        AssetBundle Load(string by);
        
        UniTask<AssetBundle> LoadAsync(string by, IProgress<float> with,
            CancellationToken cancellationToken = default);
    }
}