using System;

namespace Depra.Assets.Tests.PlayMode.Configuration
{
    internal sealed class AssetsTestException : Exception
    {
        private const string MESSAGE_FORMAT = "You need to create test config {0} in Resources folder";

        public AssetsTestException(string configName) : 
            base(string.Format(MESSAGE_FORMAT, configName)) { }
    }
}