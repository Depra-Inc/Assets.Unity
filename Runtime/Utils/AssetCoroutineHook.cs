using System.Collections;
using Depra.Coroutines.Runtime;
using UnityEngine;

namespace Depra.Assets.Runtime.Utils
{
    [RequireComponent(typeof(ICoroutineHost))]
    internal sealed class AssetCoroutineHook : MonoBehaviour, ICoroutineHost
    {
        private ICoroutineHost _coroutineHost;

        private static AssetCoroutineHook _instance;

        public static AssetCoroutineHook Instance => _instance ??= Initialize();

        private static AssetCoroutineHook Initialize()
        {
            var instance = new GameObject().AddComponent<AssetCoroutineHook>();
            DontDestroyOnLoad(instance);

            return instance;
        }

        private void Awake()
        {
            _instance = this;
            _coroutineHost = GetComponent<ICoroutineHost>();
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        ICoroutine ICoroutineHost.StartCoroutine(IEnumerator coroutine) =>
            _coroutineHost.StartCoroutine(coroutine);

        void ICoroutineHost.StopCoroutine(ICoroutine coroutine) =>
            _coroutineHost.StopCoroutine(coroutine);
    }
}