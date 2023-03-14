using System.Collections;
using System.Runtime.CompilerServices;
using Depra.Coroutines.Domain.Entities;
using Depra.Coroutines.Unity.Runtime;
using UnityEngine;

namespace Depra.Assets.Runtime.Utils
{
    [RequireComponent(typeof(ICoroutineHost))]
    internal sealed class AssetCoroutineHook : MonoBehaviour, ICoroutineHost
    {
        [SerializeField] private RuntimeCoroutineHost _runtimeHost;

        private static AssetCoroutineHook _instance;

        public static AssetCoroutineHook Instance
        {
            get => _instance ??= Initialize(Create());
            private set => Initialize(value);
        }

        internal static void Destroy()
        {
            if (_instance == null)
            {
                return;
            }
            
            if (Application.isPlaying)
            {
                Object.Destroy(_instance);
            }
            else
            {
                DestroyImmediate(_instance);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static AssetCoroutineHook Initialize(AssetCoroutineHook instance)
        {
            instance._runtimeHost = instance.GetOrAddComponent<RuntimeCoroutineHost>();
            instance.name = nameof(AssetCoroutineHook);
            //DontDestroyOnLoad(instance);
            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static AssetCoroutineHook Create() => new GameObject()
            .AddComponent<AssetCoroutineHook>();

        private void Awake() => Instance = this;

        private void OnDestroy() => _instance = null;

        ICoroutine ICoroutineHost.StartCoroutine(IEnumerator coroutine) =>
            _runtimeHost.StartCoroutine(coroutine);

        void ICoroutineHost.StopCoroutine(ICoroutine coroutine) =>
            _runtimeHost.StopCoroutine(coroutine);

        private TComponent GetOrAddComponent<TComponent>() where TComponent : Component
        {
            if (gameObject.TryGetComponent<TComponent>(out var component) == false)
            {
                component = gameObject.AddComponent<TComponent>();
            }

            return component;
        }
    }
}