using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Idents;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Structs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Resource
{
    public sealed class ResourceAsset<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : Object
    {
        private readonly AssetIdent _ident;

        private TAsset _loadedAsset;

        public static implicit operator TAsset(ResourceAsset<TAsset> asset) =>
            asset.Load();

        public ResourceAsset(AssetIdent ident) =>
            _ident = ident ?? throw new ArgumentNullException(nameof(ident));

        public string Path => _ident.Uri;
        public string Name => _ident.Name;

        public bool IsLoaded => _loadedAsset != null;
        public FileSize Size { get; private set; } = FileSize.Unknown;

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var loadedAsset = Resources.Load<TAsset>(Path);

            Guard.AgainstNull(loadedAsset, () => new ResourceNotLoadedException(Path));

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
        
        public async UniTask<TAsset> LoadAsync(CancellationToken cancellationToken,
            DownloadProgressDelegate onProgress = null)
        {
            if (IsLoaded)
            {
                onProgress?.Invoke(DownloadProgress.Full);

                return _loadedAsset;
            }

            var progress = Progress.Create<float>(value => onProgress?.Invoke(new DownloadProgress(value)));
            var loadedAsset = await Resources.LoadAsync<TAsset>(Path)
                .ToUniTask(progress, cancellationToken: cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            onProgress?.Invoke(DownloadProgress.Full);

            Guard.AgainstNull(loadedAsset, () => new ResourceNotLoadedException(Path));

            _loadedAsset = (TAsset) loadedAsset;
            Size = FileSize.FromProfiler(_loadedAsset);

            return _loadedAsset;
        }

        void IDisposable.Dispose() => Unload();
    }
}