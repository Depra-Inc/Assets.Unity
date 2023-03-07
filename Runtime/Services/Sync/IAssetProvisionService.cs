using System;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Services.Sync
{
    [PublicAPI]
    public interface IAssetProvisionService
    {
        Object GetAsset(Type assetType, string directory, string assetName);

        T GetAsset<T>(string directory, string assetName) where T : Object;

        bool TryGetAsset(Type assetType, string directory, string assetName);

        bool TryGetAsset<T>(string directory, string assetName);
    }
}