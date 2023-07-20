using Depra.Assets.ValueObjects;
using UnityEngine;
using UnityEngine.Profiling;

namespace Depra.Assets.Unity.Runtime.Common
{
    public static class UnityFileSize
    {
        public static FileSize FromProfiler(Object asset) => new(Profiler.GetRuntimeMemorySizeLong(asset));
    }
}