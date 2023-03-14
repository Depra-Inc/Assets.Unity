using System.Diagnostics.CodeAnalysis;
using Depra.Assets.Runtime.Common;
using UnityEngine;

namespace Depra.Assets.Tests.Common
{
    [CreateAssetMenu(fileName = nameof(TestAsset), menuName = "Depra/Assets/Tests/Asset", order = 51)]
    public sealed class TestAsset : ScriptableObject
    {
        public AssetIdent Ident { get; private set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public void Initialize(string name, string path) =>
            Ident = new AssetIdent(name, path);
    }
}