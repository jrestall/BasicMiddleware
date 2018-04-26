// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// Enumerates the supported hashing algorithms.
    /// </summary>
    [Flags]
    public enum HashAlgorithms
    {
        /// <summary>
        /// A SHA256 hash algorithm.
        /// </summary>
        SHA256 = 1,

        /// <summary>
        /// A SHA384 hash algorithm.
        /// </summary>
        SHA384 = 2,

        /// <summary>
        /// A SHA512 hash algorithm.
        /// </summary>
        SHA512 = 4
    }
}
