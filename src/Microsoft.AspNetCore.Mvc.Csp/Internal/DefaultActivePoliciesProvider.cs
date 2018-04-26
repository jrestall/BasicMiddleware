// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.Csp.Internal
{
    /// <inheritdoc />
    public class DefaultActivePoliciesProvider : IActivePoliciesProvider
    {
        private readonly ICspProvider _policyProvider;

        public DefaultActivePoliciesProvider(ICspProvider policyProvider)
        {
            _policyProvider = policyProvider;
        }

        /// <inheritdoc />
        public async Task<ContentSecurityPolicy> GetActiveMainPolicyAsync(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var policies = await GetActivePoliciesAsync(httpContext);
            return policies?.FirstOrDefault();
        }

        /// <inheritdoc />
        public Task<IEnumerable<ContentSecurityPolicy>> GetActivePoliciesAsync(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var activePolicies = httpContext.Items[nameof(DefaultActivePoliciesProvider)] as IEnumerable<ContentSecurityPolicy>;
            return Task.FromResult(activePolicies);
        }

        /// <inheritdoc />
        public async Task SetActivePoliciesAsync(HttpContext httpContext, IEnumerable<string> policyNames)
        {
            var activePolicies = new List<ContentSecurityPolicy>();
            if (policyNames == null)
            {
                await CopyPolicyToActiveListAsync(httpContext, activePolicies, null);     
            }
            else
            {
                foreach (var policyName in policyNames)
                {
                    await CopyPolicyToActiveListAsync(httpContext, activePolicies, policyName);
                }
            }

            httpContext.Items[nameof(DefaultActivePoliciesProvider)] = activePolicies;
        }

        private async Task CopyPolicyToActiveListAsync(HttpContext httpContext, List<ContentSecurityPolicy> activePolicies, string policyName)
        {
            var configuredPolicy = await _policyProvider.GetPolicyAsync(httpContext, policyName);
            if (configuredPolicy == null)
            {
                var errorMessage = "todo";
                /*var errorMessage = policyName == null
                    ? Resources.FormatActivePoliciesProvider_MissingDefaultCspPolicy()
                    : Resources.FormatActivePoliciesProvider_MissingCspPolicy(policyName);*/

                throw new InvalidOperationException(errorMessage);
            }

            var activePolicy = new ContentSecurityPolicy(configuredPolicy);
            activePolicies.Add(activePolicy);
        }
    }
}