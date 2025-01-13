// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.IO;
using static Depra.Assets.Internal.UnityProject;

namespace Depra.Assets.Files.Resource
{
	public sealed record ResourcesPath : IAssetUri, IEquatable<IAssetUri>
	{
		private static readonly string RESOURCES_FOLDER_PATH = RESOURCES_FOLDER_NAME + Path.AltDirectorySeparatorChar;

		public static readonly ResourcesPath Empty = new(string.Empty, string.Empty);
		public static readonly ResourcesPath Invalid = new(nameof(Invalid), string.Empty);

		public static implicit operator AssetName(ResourcesPath path) => new(path.Relative);
		public static implicit operator ResourcesPath(string relativePath) => new(relativePath, string.Empty);

		public ResourcesPath(string projectPath)
		{
			Project = projectPath;
			Absolute = Path.GetFullPath(Project);
			Relative = FindRelativePath(Project);
			Directory = new DirectoryInfo(Path.GetDirectoryName(Absolute)!);
		}

		public ResourcesPath(string relativePath, string resourcesLocation = null)
		{
			Relative = relativePath;
			Project = string.IsNullOrEmpty(resourcesLocation)
				? Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME, Relative)
				: Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME, resourcesLocation, Relative);

			Absolute = Path.GetFullPath(Project);
			Directory = new DirectoryInfo(Path.GetDirectoryName(Absolute)!);
		}

		public ResourcesPath(string name, string extension = null, string relativeDirectory = null,
			string resourcesLocation = null)
		{
			Relative = string.IsNullOrEmpty(relativeDirectory) ? name : Path.Combine(relativeDirectory, name);
			Project = string.IsNullOrEmpty(resourcesLocation)
				? Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME, Relative + extension)
				: Path.Combine(ASSETS_FOLDER_NAME, RESOURCES_FOLDER_NAME, resourcesLocation, Relative + extension);

			Absolute = Path.GetFullPath(Project);
			Directory = new DirectoryInfo(Path.GetDirectoryName(Absolute)!);
		}

		public string Relative { get; }
		public string Absolute { get; }
		public string Project { get; }
		public DirectoryInfo Directory { get; }

		internal static string FindRelativePath(string projectPath)
		{
			Guard.AgainstEmptyString(projectPath, () => new NullReferenceException(nameof(projectPath)));

			projectPath = projectPath.ToUnixPath();
			var folderIndex = projectPath.IndexOf(RESOURCES_FOLDER_PATH, StringComparison.OrdinalIgnoreCase);

			Guard.AgainstEqual(folderIndex, -1, () => new PathDoesNotContainResourcesFolder(projectPath));

			folderIndex += RESOURCES_FOLDER_PATH.Length;
			var length = projectPath.Length - folderIndex;
			length -= projectPath.Length - projectPath.LastIndexOf('.');

			return projectPath.Substring(folderIndex, length);
		}

		public override int GetHashCode() => HashCode.Combine(Absolute);

		bool IEquatable<IAssetUri>.Equals(IAssetUri other) =>
			Path.GetFileNameWithoutExtension(Relative) ==
			Path.GetFileNameWithoutExtension(other?.Relative);
	}
}