using System;
using Object = UnityEngine.Object;

namespace Depra.Assets.Runtime.Internal.Extensions
{
    internal static class ActionExtensions
    {
        public static void Invoke<TAsset>(this Action<TAsset> action, Object arg) where TAsset : Object =>
            action.Invoke((TAsset)arg);
    }
}