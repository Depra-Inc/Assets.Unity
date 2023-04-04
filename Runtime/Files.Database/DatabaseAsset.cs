using System;
using System.IO;
using System.Runtime.CompilerServices;
using Depra.Assets.Runtime.Async.Tokens;
using Depra.Assets.Runtime.Files.Interfaces;
using Depra.Assets.Runtime.Files.Structs;
using Depra.Assets.Runtime.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using static Depra.Assets.Runtime.Common.Constants;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Files.Database
{
    public sealed class DatabaseAsset<TAsset> : ILoadableAsset<TAsset>, IDisposable where TAsset : ScriptableObject
    {
        private readonly Type _assetType;
        private readonly string _absoluteFilePath;
        private readonly string _absoluteDirectoryPath;

        private TAsset _loadedAsset;

        public DatabaseAsset(string directoryPath, string name, string typeExtension = AssetTypes.BASE)
        {
            Name = name;
            _assetType = typeof(TAsset);
            _absoluteDirectoryPath = System.IO.Path.Combine(Application.dataPath, directoryPath);
            var nameWithExtension = Name + typeExtension;
            _absoluteFilePath = System.IO.Path.Combine(_absoluteDirectoryPath, nameWithExtension);
            var projectPath = System.IO.Path.Combine(ASSETS_FOLDER_NAME, directoryPath, nameWithExtension);
            Path = projectPath;
        }

        public string Name { get; }
        public string Path { get; }

        public bool IsLoaded => _loadedAsset != null;
        public FileSize Size => new(Profiler.GetRuntimeMemorySizeLong(_loadedAsset));

        public TAsset Load()
        {
            if (IsLoaded)
            {
                return _loadedAsset;
            }

            TAsset asset = null;
#if UNITY_EDITOR
            if (File.Exists(_absoluteFilePath))
            {
                asset = AssetDatabase.LoadAssetAtPath<TAsset>(Path);
            }
#endif
            if (asset == null)
            {
                asset = CreateAsset();
            }

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

        public IAsyncToken LoadAsync(Action<TAsset> onLoaded, Action<float> onProgress = null,
            Action<Exception> onFailed = null)
        {
            if (IsLoaded)
            {
                return AlreadyLoadedAsset<TAsset>.Create(_loadedAsset, onLoaded, onProgress);
            }

            var task = UnityMainThreadDispatcher.Instance().EnqueueAsync(() =>
            {
                try
                {
                    var asset = Load();
                    onProgress?.Invoke(1f);
                    onLoaded.Invoke(asset);
                }
                catch (Exception exception)
                {
                    onFailed?.Invoke(exception);
                }
            });

            return AsyncActionToken.Empty;
        }

        private TAsset CreateAsset()
        {
            var asset = ScriptableObject.CreateInstance<TAsset>();
#if UNITY_EDITOR
            asset = (TAsset)ActivateAsset(asset);
#endif

            return asset;
        }

#if UNITY_EDITOR
        private Object ActivateAsset(Object asset)
        {
            CreateFolderIfDoesNotExist();

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateFolderIfDoesNotExist()
        {
            if (Directory.Exists(_absoluteDirectoryPath) == false)
            {
                Directory.CreateDirectory(_absoluteDirectoryPath);
            }
        }

        void IDisposable.Dispose() => Unload();
    }
}