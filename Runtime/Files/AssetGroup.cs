using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files
{
    public sealed class AssetGroup : ILoadableAsset<IEnumerable<Object>>, IDisposable
    {
        private readonly List<ILoadableAsset<Object>> _children;

        public AssetGroup(string name = "", string path = "", List<ILoadableAsset<Object>> children = null)
        {
            Name = name;
            Path = path;
            _children = children ?? new List<ILoadableAsset<Object>>();
        }

        public string Name { get; }
        public string Path { get; }

        public bool IsLoaded => _children.All(asset => asset.IsLoaded);
        public FileSize Size => new(_children.Sum(x => x.Size.SizeInBytes));

        public void AddAsset(ILoadableAsset<Object> asset)
        {
            _children.Add(asset);
        }

        public IEnumerable<Object> Load()
        {
            foreach (var asset in _children)
            {
                yield return asset.Load();
            }
        }

        public IDisposable LoadAsync(Action<IEnumerable<Object>> onLoaded, Action<float> onProgress = null, Action<Exception> onFailed = null)
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            foreach (var asset in _children)
            {
                asset.Unload();
            }
        }

        void IDisposable.Dispose() => Unload();
    }
}