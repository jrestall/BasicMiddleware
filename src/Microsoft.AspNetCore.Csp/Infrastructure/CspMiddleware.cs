// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// An ASP.NET middleware for handling content security policy headers.
    /// </summary>
    public class CspMiddleware
    {
        private readonly RequestDelegate _next;
		private readonly ContentSecurityPolicy _policy;
        private readonly string _policyName;

        /// <summary>
        /// Instantiates a new <see cref="CspMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public CspMiddleware(RequestDelegate next) : this(next, policyName: null)
        {
        }

        /// <summary>
        /// Instantiates a new <see cref="CspMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="policyName">An optional name of the policy to be fetched.</param>
        public CspMiddleware(RequestDelegate next, string policyName)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _next = next;
            _policyName = policyName;
        }

        /// <summary>
        /// Instantiates a new <see cref="CspMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
		/// <param name="policy">An instance of the <see cref="ContentSecurityPolicy"/> which can be applied.</param>
        public CspMiddleware(
           RequestDelegate next,
           ContentSecurityPolicy policy)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _next = next;
            _policy = policy;
        }

        /// <summary>
        /// Invokes the <see cref="CspMiddleware"/> with the given services.
        /// </summary>
        /// <param name="httpContext">The current <see cref="HttpContext"/> for the request.</param>
        /// <param name="policyProvider">The injected policy provider which can get a <see cref="ContentSecurityPolicy"/>.</param>
        /// <param name="cspHeaderBuilder">The injected <see cref="ICspHeaderBuilder"/> that builds the header.</param>
        public async Task Invoke(HttpContext httpContext, ICspProvider policyProvider, ICspHeaderBuilder cspHeaderBuilder)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (policyProvider == null)
            {
                throw new ArgumentNullException(nameof(policyProvider));
            }

            var policy = _policy ?? await policyProvider.GetPolicyAsync(httpContext, _policyName);
            if (policy != null)
            {
                var header = cspHeaderBuilder.GetHeader(httpContext, policy);

                httpContext.Response.Headers.Append(header.Name, header.Value);
            }

            await _next(httpContext);
        }
    }
}