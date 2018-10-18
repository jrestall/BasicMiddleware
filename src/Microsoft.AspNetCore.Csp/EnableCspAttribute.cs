// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Csp.Infrastructure;

namespace Microsoft.AspNetCore.Csp
{
    /// <inheritdoc cref="IEnableCspAttribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EnableCspAttribute : Attribute, IEnableCspAttribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EnableCspAttribute"/> with the default policy
        /// name defined by <see cref="CspOptions.DefaultPolicyName"/>.
        /// </summary>
        public EnableCspAttribute()
            : this(policyNames: null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EnableCspAttribute"/> with the supplied policy names.
        /// </summary>
        /// <param name="policyNames">The name of the policies to be applied.</param>
        public EnableCspAttribute(params string[] policyNames)
        {
            PolicyNames = policyNames;
        }

        /// <inheritdoc />
        public string[] PolicyNames { get; set; }
    }
}