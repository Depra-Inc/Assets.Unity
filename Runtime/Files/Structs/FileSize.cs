// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;

namespace Depra.Assets.Runtime.Files.Structs
{
    [Serializable]
    public readonly struct FileSize : IEquatable<FileSize>
    {
        public readonly long SizeInBytes;
        public readonly double SizeInKilobytes;
        public readonly double SizeInMegabytes;

        public static FileSize Zero => new(0);
        public static FileSize Unknown => new(-1);

        public FileSize(long sizeInBytes)
        {
            SizeInBytes = sizeInBytes;
            SizeInKilobytes = (double)SizeInBytes / 1024;
            SizeInMegabytes = (double)SizeInBytes / (1024 * 1024);
        }

        public bool Equals(FileSize other) =>
            SizeInMegabytes.Equals(other.SizeInMegabytes) &&
            SizeInKilobytes.Equals(other.SizeInKilobytes) &&
            SizeInBytes == other.SizeInBytes;

        public override bool Equals(object obj) =>
            obj is FileSize other && Equals(other);

        public override int GetHashCode() => 
            HashCode.Combine(SizeInBytes, SizeInKilobytes, SizeInMegabytes);

        public override string ToString() => this.ToHumanReadableString();
    }

    public static class FileSizeExtensions
    {
        private const string SIZE_IN_BYTES_FORMAT = "{0} B";
        private const string SIZE_IN_KILOBYTES_FORMAT = "{0} KB";
        private const string SIZE_IN_MEGABYTES_FORMAT = "{0} MB";

        public static string ToHumanReadableString(this FileSize fileSize) => fileSize switch
        {
            { SizeInMegabytes: > 1 } => string.Format(SIZE_IN_MEGABYTES_FORMAT, fileSize.SizeInMegabytes),
            { SizeInKilobytes: > 1 } => string.Format(SIZE_IN_KILOBYTES_FORMAT, fileSize.SizeInKilobytes),
            _ => string.Format(SIZE_IN_BYTES_FORMAT, fileSize.SizeInBytes)
        };
    }
}