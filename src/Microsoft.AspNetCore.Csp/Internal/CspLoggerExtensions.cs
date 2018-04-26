// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Csp.Internal
{
    internal static class CspLoggerExtensions
    {
        private static readonly Action<ILogger, Exception> _policySuccess;

        static CspLoggerExtensions()
        {
            _policySuccess = LoggerMessage.Define(
                LogLevel.Information,
                1,
                "Policy successfully applied to response.");
        }

        public static void PolicySuccess(this ILogger logger)
        {
            _policySuccess(logger, null);
        }
    }
}
