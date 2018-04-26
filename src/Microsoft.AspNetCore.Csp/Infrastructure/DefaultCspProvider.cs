// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <inheritdoc />
    public class DefaultCspProvider : ICspProvider
    {
        private readonly CspOptions _options;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultCspProvider"/>.
        /// </summary>
        /// <param name="options">The options configured for the application.</param>
        public DefaultCspProvider(IOptions<CspOptions> options)
        {
            _options = options.Value;
        }

        /// <inheritdoc />
        public Task<ContentSecurityPolicy> GetPolicyAsync(HttpContext httpContext, string policyName)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            return Task.FromResult(_options.GetPolicy(policyName ?? _options.DefaultPolicyName));
        }
    }
}