using System;
using System.IO;
using Depra.Assets.Idents;
using Depra.Assets.Unity.Runtime.Exceptions;
using Depra.Assets.Unity.Runtime.Files.Resource.Exceptions;
using JetBrains.Annotations;
using static Depra.Assets.Unity.Runtime.Common.Constants;

namespace Depra.Assets.Unity.Runtime.Files.Resource
{
    public sealed class ResourcesPath : IAssetIdent
    {
        private static readonly string RESOURCES_FOLDER_PATH =
            Path.DirectorySeparatorChar +
            RESOURCES_FOLDER_NAME +
            Path.DirectorySeparatorChar;

        public static ResourcesPath Empty => new(string.Empty, string.Empty);
        public static ResourcesPath Invalid => new(string.Empty, nameof(Invalid));

        internal ResourcesPath(string name, string relativeDirectory = null, string extension = null) : this(
            Path.Combine(
                ASSETS_FOLDER_NAME,
                RESOURCES_FOLDER_NAME,
                relativeDirectory ?? string.Empty,
                name + extension)) { }

        internal ResourcesPath(string projectPath)
        {
            ProjectPath = projectPath;
            RelativePath = FindRelativePath();
            AbsolutePath = Path.GetFullPath(ProjectPath);
        }

        public string ProjectPath { get; }

        public string RelativePath { get; }

        [UsedImplicitly]
        public string AbsolutePath { get; }

        internal string FindRelativePath()
        {
            Guard.AgainstEmptyString(ProjectPath, () => new NullReferenceException(nameof(ProjectPath)));
            var folderIndex = ProjectPath.IndexOf(RESOURCES_FOLDER_PATH, StringComparison.Ordinal);
            Guard.AgainstEqual(folderIndex, -1, () => new PathDoesNotContainResourcesFolder());

            folderIndex += RESOURCES_FOLDER_PATH.Length;
            var length = ProjectPath.Length - folderIndex;
            length -= ProjectPath.Length - ProjectPath.LastIndexOf('.');

            return ProjectPath.Substring(folderIndex, length);
        }

        string IAssetIdent.Uri => AbsolutePath;

        string IAssetIdent.RelativeUri => RelativePath;
    }
}