using System;

namespace Depra.Assets.Tests.PlayMode.Exceptions
{
    internal sealed class TestAssetsConfigurationException : Exception
    {
        private const string MESSAGE_FORMAT = "No assets found by path: {0}!";

        public TestAssetsConfigurationException(string path) :
            base(string.Format(MESSAGE_FORMAT, path)) { }
    }
}