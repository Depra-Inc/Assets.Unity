using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Delegates;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.ValueObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Resource
{
    public sealed class ResourceAsset<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
    {
        private readonly ResourceIdent _ident;
        private TAsset _loadedAsset;

        public static implicit operator TAsset(ResourceAsset<TAsset> asset) =>
            asset.Load();

        public ResourceAsset(ResourceIdent ident) =>
            _ident = ident ?? throw new ArgumentNullException(nameof(ident));

        public IAssetIdent Ident => _ident;
        public bool IsLoaded => _loadedAsset != null;
        public FileSize Size { get; private set; } = FileSize.Unknown;

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var loadedAsset = Resources.Load<TAsset>(_ident.RelativePath);

            Guard.AgainstNull(loadedAsset, () => new ResourceNotLoadedException(_ident.RelativePath));

            _loadedAsset = loadedAsset;
            Size = FileSize.FromProfiler(_loadedAsset);

            return loadedAsset;
        }

        public void Unload()
        {
            if (IsLoaded == false)
            {
                return;
            }

            Resources.UnloadAsset(_loadedAsset);
            _loadedAsset = null;
        }

        public async UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
            CancellationToken cancellationToken = default)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);
                return _loadedAsset;
            }

            var progress = Progress.Create<float>(value => onProgress?.Invoke(new DownloadProgress(value)));
            var loadedAsset = await Resources.LoadAsync<TAsset>(_ident.RelativePath)
                .ToUniTask(progress, cancellationToken: cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            onProgress?.Invoke(DownloadProgress.Full);

            Guard.AgainstNull(loadedAsset, () => new ResourceNotLoadedException(_ident.RelativePath));

            _loadedAsset = (TAsset) loadedAsset;
            Size = FileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        void IDisposable.Dispose() => Unload();
    }
}