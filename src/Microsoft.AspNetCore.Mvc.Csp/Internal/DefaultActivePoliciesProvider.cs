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

            var activePolicies = httpContext.Items[nameof(DefaultActivePoliciesProvider)] as Dictionary<string, ContentSecurityPolicy>;
            if (activePolicies == null)
            {
                return Task.FromResult(Enumerable.Empty<ContentSecurityPolicy>());
            }
            
            return Task.FromResult(activePolicies.Select(x => x.Value));
        }

        /// <inheritdoc />
        public async Task SetActivePoliciesAsync(HttpContext httpContext, IEnumerable<string> policyNames)
        {
            var activePolicies = new Dictionary<string, ContentSecurityPolicy>();
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

        private async Task CopyPolicyToActiveListAsync(HttpContext httpContext, Dictionary<string, ContentSecurityPolicy> activePolicies, string policyName)
        {
            var configuredPolicy = await _policyProvider.GetPolicyAsync(httpContext, policyName);
            if (configuredPolicy == null)
            {
                // TODO: Proper error message.
                var errorMessage = "CSP was enabled but no default content security policy has been configured.";
                /*var errorMessage = policyName == null
                    ? Resources.FormatActivePoliciesProvider_MissingDefaultCspPolicy()
                    : Resources.FormatActivePoliciesProvider_MissingCspPolicy(policyName);*/

                throw new InvalidOperationException(errorMessage);
            }

            var activePolicy = new ContentSecurityPolicy(configuredPolicy);
            activePolicies.Add(policyName ?? "__DefaultContentSecurityPolicy", activePolicy);
        }
    }
}