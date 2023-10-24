// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
using Depra.Assets.ValueObjects;
using JetBrains.Annotations;

namespace Depra.Assets.Files.Database
{
	public readonly struct DatabaseAssetUri : IAssetUri
	{
		public static DatabaseAssetUri Empty => new();
		public static implicit operator DatabaseAssetUri(string relativePath) => new(relativePath);

		public DatabaseAssetUri(string relativePath) : this(Path.GetDirectoryName(relativePath),
			Path.GetFileNameWithoutExtension(relativePath),
			Path.GetExtension(relativePath)) { }

		public DatabaseAssetUri(string relativeDirectory, string name, string extension)
		{
			Name = name;
			Extension = extension;
			NameWithExtension = Name + Extension;
			RelativePath = Path.Combine(relativeDirectory, NameWithExtension).Replace(@"\", "/");
			AbsolutePath = Path.GetFullPath(RelativePath).Replace(@"\", "/");
			AbsoluteDirectoryPath = Path.GetFullPath(relativeDirectory).Replace(@"\", "/");
			Directory = new DirectoryInfo(AbsoluteDirectoryPath);
		}

		public string Name { get; }

		[UsedImplicitly]
		public string Extension { get; }

		[UsedImplicitly]
		public string NameWithExtension { get; }

		public string RelativePath { get; }
		public string AbsolutePath { get; }

		[UsedImplicitly]
		public string AbsoluteDirectoryPath { get; }

		internal DirectoryInfo Directory { get; }

		string IAssetUri.Relative => RelativePath;
		string IAssetUri.Absolute => AbsolutePath;

		public bool Exists() => File.Exists(AbsolutePath);
	}
}