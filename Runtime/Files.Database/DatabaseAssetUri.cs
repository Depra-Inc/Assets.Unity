// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;

namespace Depra.Assets.Files.Database
{
	public sealed record DatabaseAssetUri : IAssetUri
	{
		public static DatabaseAssetUri Empty => new(string.Empty);
		public static implicit operator DatabaseAssetUri(string relativePath) => new(relativePath);

		public DatabaseAssetUri(string relativePath) : this(Path.GetDirectoryName(relativePath),
			Path.GetFileNameWithoutExtension(relativePath),
			Path.GetExtension(relativePath)) { }

		public DatabaseAssetUri(string relativeDirectory, string name, string extension)
		{
			Name = name;
			Extension = extension;
			NameWithExtension = Name + Extension;
			Relative = Path.Combine(relativeDirectory, NameWithExtension).Replace(@"\", "/");
			Absolute = Path.GetFullPath(Relative).Replace(@"\", "/");
			AbsoluteDirectory = Path.GetFullPath(relativeDirectory).Replace(@"\", "/");
			Directory = new DirectoryInfo(AbsoluteDirectory);
		}

		public string Name { get; }
		public string Extension { get; }
		public string NameWithExtension { get; }

		public string Relative { get; }
		public string Absolute { get; }

		public string AbsoluteDirectory { get; }
		internal DirectoryInfo Directory { get; }

		public bool Exists() => File.Exists(Absolute);

		public override string ToString() => Absolute;
	}
}