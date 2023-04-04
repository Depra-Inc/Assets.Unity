using System.IO;
using Depra.Assets.Runtime.Files.Interfaces;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Extensions
{
    public static class LoadableAssetFileExtensions
    {
        public static string ReadTextFromFile(this ILoadableAsset<TextAsset> file)
        {
            var textAsset = file.Load();
            if (textAsset == null)
            {
                throw new FileNotFoundException($"The file you specified doesn't exist in {nameof(Resources)}!");
            }

            var text = textAsset.text;
            file.Unload();

            return text;
        }
    }
}