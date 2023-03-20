using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Coroutines.Domain.Entities;

namespace Depra.Assets.Runtime.Files.Bundles.Files
{
    public sealed class AssetFileLoadingCoroutine : IDisposable
    {
        private readonly ICoroutineHost _coroutineHost;
        private ICoroutine _loadCoroutine;

        public AssetFileLoadingCoroutine(ICoroutineHost coroutineHost) => _coroutineHost =
            coroutineHost ?? throw new ArgumentNullException(nameof(coroutineHost));

        public bool Done => _loadCoroutine == null;
        public bool Running => _loadCoroutine != null;

        public void Start(IEnumerator loadingProcess) =>
            _loadCoroutine = _coroutineHost.StartCoroutine(loadingProcess);

        public void Cancel()
        {
            if (Running)
            {
                Clean();
            }
        }

        public void Destroy()
        {
            if (Done || Running)
            {
                Clean();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Clean()
        {
            _coroutineHost.StopCoroutine(_loadCoroutine);
            _loadCoroutine = null;
        }

        void IDisposable.Dispose() => Clean();
    }
}