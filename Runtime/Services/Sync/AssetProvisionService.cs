using System;
using System.Collections.Generic;
using System.IO;
using Depra.Assets.Runtime.Factory;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Services.Sync
{
    public class AssetProvisionService : IAssetProvisionService
    {
        //private readonly AssetLoader _loader;
        private readonly AssetFactory _factory;

        private readonly Dictionary<Type, Object> _cache;
        private readonly object _instanceLock = new();

        public AssetProvisionService( AssetFactory factory = null)
        {
           // _loader = loader;
            _factory = factory ?? new ScriptableObjectFactory();

            _cache = new Dictionary<Type, Object>();
        }

        public T GetAsset<T>(string directory, string assetName) where T : Object
        {
            return (T)GetAsset(typeof(T), directory, assetName);
        }

        public bool TryGetAsset(Type assetType, string directory, string assetName)
        {
            throw new NotImplementedException();
        }

        public bool TryGetAsset<T>(string directory, string assetName)
        {
            throw new NotImplementedException();
        }

        public Object GetAsset(Type assetType, string directory, string assetName)
        {
            if (TryLoadAsset(assetType, directory, assetName, out var asset))
            {
                return asset;
            }

            if (asset == null && _factory == null)
            {
                throw new NullReferenceException();
            }

            Debug.Log($"Asset {assetName} was not found and will be created in {directory}.");

            asset = _factory.CreateAsset(assetType, directory, assetName);
            AddToCache(assetType, asset);

            return asset;
        }

        public void ClearCache()
        {
            lock (_instanceLock)
            {
                _cache.Clear();
            }
        }

        public bool TryLoadAsset<T>(string directory, string assetName, out T asset) where T : Object
        {
            var type = typeof(T);
            if (TryLoadAsset(type, directory, assetName, out var nonCastedAsset) == false)
            {
                throw new Exception($"Cannot be converted to type '{type}'");
            }

            asset = (T)nonCastedAsset;
            return true;
        }

        private bool TryLoadAsset(Type type, string directory, string assetName, out Object asset)
        {
            lock (_instanceLock)
            {
                // For fast enter play mode.
                if (TryGetInstanceFromCache(type, out asset) && asset != null)
                {
                    return true;
                }

                //asset = _loader.LoadAsset<Object>(directory, assetName);

                if (asset == null)
                {
                    var fullPath = MakeAssetPath(directory, assetName);
                    Debug.LogWarning($"{type.FullName} not found on path {fullPath}!");
                    return false;
                }

                if (asset == null)
                {
                    return false;
                }

                _cache.Add(type, asset);

                return true;
            }
        }

        private void AddToCache(Type type, Object asset)
        {
            lock (_instanceLock)
            {
                if (_cache.TryGetValue(type, out var otherAsset) && otherAsset == asset)
                {
                    return;
                }

                _cache.Add(type, asset);
            }
        }

        private bool TryGetInstanceFromCache(Type type, out Object result)
        {
            if (_cache.ContainsKey(type))
            {
                result = _cache[type];
                if (result)
                {
                    return true;
                }

                _cache.Remove(type);
                return false;
            }

            result = default;
            return false;
        }

        private static string MakeAssetPath(string directory, string assetName)
        {
            var fullPath = Path.Combine(directory, assetName);
            return fullPath;
        }
    }
}