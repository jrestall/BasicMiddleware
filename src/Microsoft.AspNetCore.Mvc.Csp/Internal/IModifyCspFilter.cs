// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.Csp.Internal
{
    /// <summary>
    /// A filter that modifies content security policies.
    /// </summary>
    public interface IModifyCspFilter : IFilterMetadata
    {
        /// <summary>
        /// The name of the policy modifier.
        /// </summary>
        string PolicyName { get; }

        /// <summary>
        /// A comma separated list of the policy names which will be appended to. Defaults to the first active policy if none provided.
        /// </summary>
        string Targets { get; }

        /// <summary>
        /// True if the policy should override directives instead of appending; otherwise false.
        /// </summary>
        bool OverrideDirectives { get; }
    }
}