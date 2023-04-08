// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.IO;
using Depra.Assets.Runtime.Common;

namespace Depra.Assets.Tests.PlayMode.Exceptions
{
    internal sealed class TestReferenceNotFoundException : Exception
    {
        private const string MESSAGE_FORMAT =
            "No {0} found! You need to create test config {1} in Resources folder!";

        public TestReferenceNotFoundException(string referenceName) :
            base(string.Format(MESSAGE_FORMAT, referenceName,
                Path.Combine(Constants.FRAMEWORK_NAME, Constants.MODULE_NAME, referenceName))) { }
    }
}