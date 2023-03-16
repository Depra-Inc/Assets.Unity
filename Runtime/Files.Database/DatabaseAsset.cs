using System;
using System.IO;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Abstract.Loading;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Database
{
    public sealed class DatabaseAsset<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : ScriptableObject
    {
        private readonly Type _assetType;
        private readonly string _absoluteDirectoryPath;

        private TAsset _loadedAsset;

        public DatabaseAsset(string path, string name, string typeExtension = AssetTypes.BASE)
        {
            Name = name;
            _assetType = typeof(TAsset);
            Path = System.IO.Path.Combine(path, Name + typeExtension);
            _absoluteDirectoryPath = System.IO.Path.Combine(Application.dataPath, path);
        }

        public string Name { get; }
        public string Path { get; }
        public bool IsLoaded => _loadedAsset != null;

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            var asset = ScriptableObject.CreateInstance<TAsset>();
#if UNITY_EDITOR
            asset = (TAsset)CreateAsset(asset);
#endif

            EnsureAsset(asset);
            _loadedAsset = asset;

            return _loadedAsset;
        }

        public void Unload()
        {
            if (IsLoaded == false)
            {
                return;
            }

#if UNITY_EDITOR
            AssetDatabase.DeleteAsset(Path);
#endif
            _loadedAsset = null;
        }

        public IDisposable LoadAsync(IAssetLoadingCallbacks<TAsset> callbacks)
        {
            throw new NotImplementedException();
        }

#if UNITY_EDITOR
        private Object CreateAsset(Object asset)
        {
            if (Directory.Exists(_absoluteDirectoryPath) == false)
            {
                Directory.CreateDirectory(_absoluteDirectoryPath);
            }

            asset.name = Name;
            AssetDatabase.CreateAsset(asset, Path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureAsset(Object asset)
        {
            if (asset == null)
            {
                throw new AssetCreationException(_assetType, _assetType.Name);
            }
        }

        void IDisposable.Dispose() => Unload();
    }
}