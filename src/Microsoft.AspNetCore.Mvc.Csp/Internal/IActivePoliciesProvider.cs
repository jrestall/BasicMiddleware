// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.Csp.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public interface IActivePoliciesProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Task<ContentSecurityPolicy> GetActiveMainPolicyAsync(HttpContext httpContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Task<IEnumerable<ContentSecurityPolicy>> GetActivePoliciesAsync(HttpContext httpContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="policyNames"></param>
        /// <returns></returns>
        Task SetActivePoliciesAsync(HttpContext httpContext, IEnumerable<string> policyNames);
    }
}