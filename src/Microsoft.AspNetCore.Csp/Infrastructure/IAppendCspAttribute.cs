// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// An interface which can be used to identify a type which provides metadata needed for appending to content security policies.
    /// </summary>
    public interface IAppendCspAttribute
    {
        /// <summary>
        /// The name of the policy which will be appended.
        /// </summary>
        string PolicyName { get; set; }

        /// <summary>
        /// A comma separated list of the policy names which will be appended to. Defaults to the first active policy if none provided.
        /// </summary>
        string Targets { get; set; }
    }
}