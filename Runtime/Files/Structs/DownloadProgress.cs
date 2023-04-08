using System;

namespace Depra.Assets.Runtime.Files.Structs
{
    [Serializable]
    public readonly struct DownloadProgress : IEquatable<DownloadProgress>
    {
        private const float MIN_VALUE = 0f;
        private const float MAX_VALUE = 1f;

        public readonly float NormalizedValue;

        public static DownloadProgress Full => new(MAX_VALUE);

        public static DownloadProgress Zero => new(MIN_VALUE);

        public DownloadProgress(float normalizedValue) => 
            NormalizedValue = normalizedValue;

        public bool Equals(DownloadProgress other) =>
            NormalizedValue.Equals(other.NormalizedValue);

        public override bool Equals(object obj) =>
            obj is DownloadProgress other && Equals(other);

        public override int GetHashCode() => NormalizedValue.GetHashCode();
    }
}