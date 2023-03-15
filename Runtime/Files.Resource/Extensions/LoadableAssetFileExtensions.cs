using System.IO;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Resource.Extensions
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