using System;
using System.Diagnostics;
using System.IO;

namespace Depra.Assets.Unity.Tests.PlayMode.Utils
{
    internal static class TypeUtility
    {
        public static string GetSourceDirectoryName(Type type)
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();
            if (frames == null)
            {
                throw new Exception($"Can't found directory for {type.Name}!");
            }

            foreach (var frame in frames)
            {
                if (frame.GetMethod() is { } method && method.DeclaringType == type)
                {
                    return Path.GetDirectoryName(frame.GetFileName());
                }
            }

            throw new Exception($"Can't found directory for {type.Name}!");
        }
    }
}