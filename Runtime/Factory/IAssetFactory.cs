using System;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Factory
{
    [PublicAPI]
    public interface IAssetFactory
    {
        Object CreateAsset(Type type, string directory, string assetName, string typeExtension = ".asset");

        T CreateAsset<T>(string directory, string assetName, string typeExtension = ".asset") where T : Object;

        void DestroyAsset(Object asset);
    }
}