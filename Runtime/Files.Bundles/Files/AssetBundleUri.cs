// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.IO;
//using Depra.Assets.Extensions;
using Depra.Asset.ValueObjects;

namespace Depra.Asset.Files.Bundles
{
	public sealed record AssetBundleUri : IAssetUri
	{
		private const string EXTENSION = ".assetbundle";

		public static AssetBundleUri Empty => new(string.Empty);
		public static AssetBundleUri Invalid => new(nameof(Invalid));
		public static implicit operator AssetBundleUri(string path) => new(path);

		private readonly FileInfo _fileInfo;

		public AssetBundleUri(string path)
		{
			_fileInfo = new FileInfo(path);
			//_fileInfo.Directory.Require();

			Name = string.IsNullOrEmpty(Extension)
				? _fileInfo.Name
				: _fileInfo.Name.Replace(Extension, string.Empty);

			AbsolutePathWithoutExtension = Absolute.Replace(EXTENSION, string.Empty);
		}

		public AssetBundleUri(string name, string directory = null) : this(name, directory, EXTENSION) { }

		public AssetBundleUri(string name, string directory, string extension = null) : this(
			Path.Combine(directory, name + extension)) { }

		public string Name { get; }
		public string Extension => EXTENSION;
		public string NameWithExtension => Name + Extension;

		public string Absolute => _fileInfo.FullName;
		public string AbsoluteDirectoryPath => _fileInfo.DirectoryName;
		public string AbsolutePathWithoutExtension { get; }

		string IAssetUri.Relative => Name;
	}
}