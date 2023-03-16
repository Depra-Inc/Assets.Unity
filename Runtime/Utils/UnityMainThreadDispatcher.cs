using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Depra.Assets.Runtime.Utils
{
    /// <summary>
    /// A thread-safe class which holds a queue with actions to execute on the next <see cref="Update"/> method.
    /// </summary>
    public sealed class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static UnityMainThreadDispatcher _instance;
        private static readonly Queue<Action> _executionQueue = new();

        private void Awake()
        {
            if (_instance != null)
            {
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Update()
        {
            lock (_executionQueue)
            {
                while (_executionQueue.Count > 0)
                {
                    _executionQueue.Dequeue().Invoke();
                }
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public static bool Exists() => _instance != null;

        public static UnityMainThreadDispatcher Instance()
        {
            if (Exists() == false)
            {
                throw new Exception(
                    "UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
            }

            return _instance;
        }

        private static IEnumerator ActionWrapper(Action action)
        {
            action();
            yield return null;
        }

        /// <summary>
        /// Locks the queue and adds the <see cref="IEnumerator"/> to the queue.
        /// </summary>
        /// <param name="action"><see cref="IEnumerator"/> function that will be executed from the main thread.</param>
        public void Enqueue(IEnumerator action)
        {
            lock (_executionQueue)
            {
                _executionQueue.Enqueue(() => StartCoroutine(action));
            }
        }

        /// <summary>
        /// Locks the queue and adds the <see cref="Action"/> to the queue.
        /// </summary>
        /// <param name="action">Function that will be executed from the main thread.</param>
        public void Enqueue(Action action) => Enqueue(ActionWrapper(action));

        /// <summary>
        /// Locks the queue and adds the <see cref="Action"/> to the queue,
        /// returning a <see cref="Task"/> which is completed when the action completes.
        /// </summary>
        /// <param name="action">Function that will be executed from the main thread.</param>
        /// <returns>A <see cref="Task"/> that can be awaited until the action completes</returns>
        public Task EnqueueAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            void WrappedAction()
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }

            Enqueue(ActionWrapper(WrappedAction));
            return tcs.Task;
        }
    }
}