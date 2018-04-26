// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// A type which can provide a <see cref="ContentSecurityPolicy"/> for a particular <see cref="HttpContext"/>.
    /// </summary>
    public interface ICspProvider
    {
        /// <summary>
        /// Gets a <see cref="ContentSecurityPolicy"/> for the given <paramref name="httpContext"/>
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> associated with this call.</param>
        /// <param name="policyName">An optional policy name to look for.</param>
        /// <returns>A <see cref="ContentSecurityPolicy"/>.</returns>
        Task<ContentSecurityPolicy> GetPolicyAsync(HttpContext httpContext, string policyName = null);
    }
}