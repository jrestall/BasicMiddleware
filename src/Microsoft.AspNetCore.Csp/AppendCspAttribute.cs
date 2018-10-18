// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Csp.Infrastructure;

namespace Microsoft.AspNetCore.Csp
{
    /// <inheritdoc cref="IAppendCspAttribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AppendCspAttribute : Attribute, IAppendCspAttribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AppendCspAttribute"/> with the supplied policy name.
        /// </summary>
        /// <param name="policyName">The name of the policy to be appended.</param>
        public AppendCspAttribute(string policyName)
        {
            PolicyName = policyName;
        }

        /// <inheritdoc />
        public string PolicyName { get; set; }

        /// <inheritdoc />
        public string Targets { get; set; }
    }
}
