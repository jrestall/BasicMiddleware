// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Csp.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.AspNetCore.Mvc.Csp
{
    /// <summary>
    /// A filter that applies the given <see cref="ContentSecurityPolicy"/> and adds appropriate response headers.
    /// </summary>
    public class CspAuthorizationFilter : ICspAuthorizationFilter
    {
        private readonly ICspProvider _policyProvider;
        private readonly IActivePoliciesProvider _activePolicyProvider;
        private readonly ICspHeaderBuilder _cspHeaderBuilder;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of <see cref="CspAuthorizationFilter"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="ICspProvider"/>.</param>
        /// <param name="activePolicyProvider"></param>
        /// <param name="cspHeaderBuilder">The <see cref="ICspHeaderBuilder"/> that builds the header.</param>
        public CspAuthorizationFilter(ICspProvider policyProvider, IActivePoliciesProvider activePolicyProvider, ICspHeaderBuilder cspHeaderBuilder)
            : this(policyProvider, activePolicyProvider, cspHeaderBuilder, NullLoggerFactory.Instance)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="CspAuthorizationFilter"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="ICspProvider"/>.</param>
        /// <param name="activePolicyProvider"></param>
        /// <param name="cspHeaderBuilder">The <see cref="ICspHeaderBuilder"/> that builds the header.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public CspAuthorizationFilter(
            ICspProvider policyProvider,
            IActivePoliciesProvider activePolicyProvider,
            ICspHeaderBuilder cspHeaderBuilder,
            ILoggerFactory loggerFactory)
        {
            if (policyProvider == null)
            {
                throw new ArgumentNullException(nameof(policyProvider));
            }

            if (activePolicyProvider == null)
            {
                throw new ArgumentNullException(nameof(activePolicyProvider));
            }

            if (cspHeaderBuilder == null)
            {
                throw new ArgumentNullException(nameof(cspHeaderBuilder));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _policyProvider = policyProvider;
            _activePolicyProvider = activePolicyProvider;
            _cspHeaderBuilder = cspHeaderBuilder;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        /// <summary>
        /// The policy names used to fetch the active list of <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        public string[] PolicyNames { get; set; }

        /// <inheritdoc />
        // Since this filter sets the active content security policies it should run early in the pipeline.
        public int Order => int.MinValue + 80;

        /// <inheritdoc />
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // If this filter is not closest to the action, it is not applicable.
            if (!context.IsEffectivePolicy<ICspAuthorizationFilter>(this))
            {
                _logger.NotMostEffectiveFilter(typeof(ICspAuthorizationFilter));
                return;
            }

            // Skip returning of content security policy headers when disable csp filter is applied.
            if (context.Filters.Any(item => item is IDisableCspFilter))
            {
                return;
            }

            var modifyFilters = context.Filters
                .Where(item => item is IModifyCspFilter)
                .Cast<IModifyCspFilter>()
                .ToList();

            // Before the action method processes, we set the current security policies for this request.
            await _activePolicyProvider.SetActivePoliciesAsync(context.HttpContext, PolicyNames);

            if (!context.HttpContext.Response.HasStarted)
            {
                // Set the HTTP header just as the response is being sent, so that other
                // code has a chance to modify the policies before the final header is sent. 
                context.HttpContext.Response.OnStarting(async state =>
                    {
                        var httpContext = (HttpContext) state;

                        // Get active policies for current response
                        var activePolicies = httpContext.Items[nameof(DefaultActivePoliciesProvider)] as IDictionary<string, ContentSecurityPolicy>;
                        if (activePolicies == null)
                        {
                            return;
                        }

                        // Modify active policies with any append & override filters
                        await ModifyActivePolicies(activePolicies, modifyFilters, httpContext);

                        // Output the Content-Security-Policy headers
                        foreach (var policy in activePolicies)
                        {
                            var header = _cspHeaderBuilder.GetHeader(context.HttpContext, policy.Value);
                            context.HttpContext.Response.Headers.Append(header.Name, header.Value);
                        }
                    },
                    state: context.HttpContext);
            }

            // Continue with other filters and action.
        }

        private async Task ModifyActivePolicies(
            IDictionary<string, ContentSecurityPolicy> activePolicies,
            IEnumerable<IModifyCspFilter> modifyFilters,
            HttpContext httpContext)
        {
            foreach (var modifyFilter in modifyFilters)
            {
                var appendPolicy = await _policyProvider.GetPolicyAsync(httpContext, modifyFilter.PolicyName);
                if (appendPolicy == null)
                {
                    // TODO: Throw error since this is an invalid configuration.
                    return;
                }

                var targetPolicies = GetTargetPolicies(activePolicies, modifyFilter.Targets);
                foreach (var targetPolicy in targetPolicies)
                {
                    targetPolicy.Copy(appendPolicy, modifyFilter.OverrideDirectives);
                }
            }
        }

        private IEnumerable<ContentSecurityPolicy> GetTargetPolicies(IDictionary<string, ContentSecurityPolicy> activePolicies, string targets)
        {
            // When no targets are provided we target the first active policy.
            if (string.IsNullOrEmpty(targets))
            {
                return activePolicies
                    .Take(1)
                    .Select(x => x.Value);             
            }

            var policyNames = targets.Split(',').Select(t => t.Trim()).ToList();
            return activePolicies
                .Where(policy => policyNames.Contains(policy.Key))
                .Select(x => x.Value);
        }
    }
}
