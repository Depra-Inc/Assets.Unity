using System;
using System.Diagnostics;
using System.IO;

namespace Depra.Assets.Tests.PlayMode.Utils
{
    public static class Extensions
    {
        public static string GetSourceDirectoryName(this Type type)
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();

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