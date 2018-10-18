// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Mvc.Csp.Internal
{
    /// <summary>
    /// An implementation of <see cref="IDisableCspFilter"/>
    /// </summary>
    public class ModifyCspFilter : IModifyCspFilter
    {
        /// <inheritdoc />
        public string PolicyName { get; }

        /// <inheritdoc />
        public string Targets { get; }

        /// <inheritdoc />
        public bool OverrideDirectives { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ModifyCspFilter"/>.
        /// </summary>
        /// <param name="policyName">The policy name to be used.</param>
        /// <param name="targets">The names of the target policies.</param>
        /// <param name="overrideDirectives">Whether the policy should override directives instead of appending.</param>
        public ModifyCspFilter(string policyName, string targets, bool overrideDirectives)
        {
            PolicyName = policyName;
            Targets = targets;
            OverrideDirectives = overrideDirectives;
        }
    }
}