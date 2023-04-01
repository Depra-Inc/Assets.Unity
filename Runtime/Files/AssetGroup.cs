using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Runtime.Async.Tokens;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files
{
    public sealed class AssetGroup : ILoadableAsset<IEnumerable<Object>>, IDisposable
    {
        private readonly List<ILoadableAsset<Object>> _childAssets;
        private List<Object> _loadedAssets;

        public AssetGroup(string name = "", string path = "", List<ILoadableAsset<Object>> children = null)
        {
            Name = name;
            Path = path;
            _loadedAssets = new List<Object>();
            _childAssets = children ?? new List<ILoadableAsset<Object>>();
        }

        public string Name { get; }
        public string Path { get; }

        public bool IsLoaded => _childAssets.All(asset => asset.IsLoaded);
        public FileSize Size => new(_childAssets.Sum(x => x.Size.SizeInBytes));

        public void AddAsset(ILoadableAsset<Object> asset)
        {
            _childAssets.Add(asset);
        }

        public IEnumerable<Object> Load()
        {
            foreach (var asset in _childAssets)
            {
                yield return asset.Load();
            }
        }

        public IAsyncToken LoadAsync(Action<IEnumerable<Object>> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            var tokenSource = new CancellationTokenSource();
            var loadingTask = Task.Run(async () =>
            {
                var loadedObjects = new List<Object>();
                var progress = 0f;

                try
                {
                    foreach (var childAsset in _childAssets)
                    {
                        // Вызываем метод загрузки дочернего ассета и получаем его результат.
                        var loadedChildAsset = await Task.FromResult(childAsset.Load());
                        loadedObjects.Add(loadedChildAsset);

                        // Вызываем колбэк прогресса, если он задан.
                        if (onProgress == null)
                        {
                            continue;
                        }
                        
                        progress += 1f / _childAssets.Count;
                        onProgress(progress);
                    }

                    // Вызываем колбэк загрузки с результатом загрузки дочерних ассетов.
                    onLoaded(loadedObjects);
                }
                catch (Exception ex)
                {
                    onFailed?.Invoke(ex);
                }

            }, tokenSource.Token);

            return new AsyncActionToken(tokenSource.Cancel);
        }

        public void Unload()
        {
            _loadedAssets.Clear();
            foreach (var asset in _childAssets)
            {
                asset.Unload();
            }
        }

        void IDisposable.Dispose() => Unload();
    }
}