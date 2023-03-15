using Depra.Assets.Runtime.Utils;
using Depra.Assets.Tests.Common;
using Depra.Coroutines.Domain.Entities;
using UnityEngine;

namespace Depra.Assets.Tests.PlayMode.Utils
{
    internal static class Create
    {
        public static ICoroutineHost RuntimeCoroutineHost() =>
            AssetCoroutineHook.Instance;
            //new GameObject().AddComponent<RuntimeCoroutineHost>();

        public static TestAsset ResourceAssetFile()
        {
            var resources = Load.Resources();
            var testAsset = (TestAsset)ScriptableObject.CreateInstance(typeof(TestAsset));
            testAsset.Initialize(resources.AssetName, resources.DirectoryPath);

            return testAsset;
        }
    }
}