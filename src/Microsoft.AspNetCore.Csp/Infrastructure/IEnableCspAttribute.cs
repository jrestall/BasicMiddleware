// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// An interface which can be used to identify a type which provides metadata needed for enabling content security policies.
    /// </summary>
    public interface IEnableCspAttribute
    {
        /// <summary>
        /// The name of the policies which need to be applied.
        /// </summary>
        string[] PolicyNames { get; set; }
    }
}