using System;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Services.Sync
{
    [PublicAPI]
    public interface IAssetService
    {
        Object GetAsset(Type assetType, string directory, string assetName);

        T GetAsset<T>(string directory, string assetName) where T : Object;
    }
}