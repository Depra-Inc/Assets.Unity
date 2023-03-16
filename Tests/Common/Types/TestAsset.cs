using System.Diagnostics.CodeAnalysis;
using Depra.Assets.Runtime.Common;
using UnityEngine;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Tests.Common.Types
{
    [CreateAssetMenu(menuName = FRAMEWORK_NAME + "/" + MODULE_NAME + "/" + nameof(TestAsset), order = 51)]
    public sealed class TestAsset : ScriptableObject
    {
        public AssetIdent Ident { get; private set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public void Initialize(string name, string path) =>
            Ident = new AssetIdent(name, path);
    }
}