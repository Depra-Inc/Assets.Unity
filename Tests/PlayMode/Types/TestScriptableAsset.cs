using System.Diagnostics.CodeAnalysis;
using Depra.Assets.Runtime.Files.Structs;
using UnityEngine;
using static Depra.Assets.Runtime.Common.Constants;

namespace Depra.Assets.Tests.PlayMode.Types
{
    [CreateAssetMenu(menuName = FRAMEWORK_NAME + "/" + MODULE_NAME + "/" + nameof(TestScriptableAsset), order = 51)]
    public sealed class TestScriptableAsset : ScriptableObject
    {
        public AssetIdent Ident { get; private set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public void Initialize(string name, string path) =>
            Ident = new AssetIdent(name, path);
    }
}