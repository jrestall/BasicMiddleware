// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
    public class CspActionFilter : ICspActionFilter
    {
        // Internal for unit testing
        internal static readonly object CspFilterContextKey = new object();

        private readonly IActivePoliciesProvider _policyProvider;
        private readonly ICspHeaderBuilder _cspHeaderBuilder;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of <see cref="CspActionFilter"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="ICspProvider"/>.</param>
        /// <param name="cspHeaderBuilder">The <see cref="ICspHeaderBuilder"/> that builds the header.</param>
        public CspActionFilter(IActivePoliciesProvider policyProvider, ICspHeaderBuilder cspHeaderBuilder)
            : this(policyProvider, cspHeaderBuilder, NullLoggerFactory.Instance)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="CspActionFilter"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="ICspProvider"/>.</param>
        /// <param name="cspHeaderBuilder">The <see cref="ICspHeaderBuilder"/> that builds the header.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public CspActionFilter(
            IActivePoliciesProvider policyProvider,
            ICspHeaderBuilder cspHeaderBuilder,
            ILoggerFactory loggerFactory)
        {
            if (policyProvider == null)
            {
                throw new ArgumentNullException(nameof(policyProvider));
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

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // If this filter is not closest to the action, it is not applicable.
            if (!context.IsEffectivePolicy<ICspActionFilter>(this))
            {
                _logger.NotMostEffectiveFilter(typeof(ICspActionFilter));
                return;
            }

            // Skip returning of content security policy headers when disable csp filter is applied.
            if (context.Filters.Any(item => item is IDisableCspFilter))
            {
                return;
            }

            // Before the action method processes, we set the current security policies for this request.
            await _policyProvider.SetActivePoliciesAsync(context.HttpContext, PolicyNames);

            if (!context.HttpContext.Response.HasStarted)
            {
                if (!context.HttpContext.Items.ContainsKey(CspFilterContextKey))
                {
                    var cspSendHeadersContext = new CspSendHeadersContext
                    {
                        HeaderBuilder = _cspHeaderBuilder,
                        PolicyProvider = _policyProvider,
                    };
                    context.HttpContext.Items.Add(CspFilterContextKey, cspSendHeadersContext);
                }

                // Register a callback before the final response headers get sent
                // so that we can add the content security policy headers.
                context.HttpContext.Response.OnStarting(async state =>
                    {
                        var httpContext = (HttpContext)state;

                        var sendHeadersContext = GetSendHeadersContext(httpContext);
                        if (sendHeadersContext == null)
                        {
                            return;
                        }

                        var policies = await sendHeadersContext.PolicyProvider.GetActivePoliciesAsync(httpContext);
                        if (policies == null)
                        {
                            return;
                        }

                        foreach (var policy in policies)
                        {
                            var header = sendHeadersContext.HeaderBuilder.GetHeader(httpContext, policy);
                            httpContext.Response.Headers.Append(header.Name, header.Value);
                        }
                    },
                    state: context.HttpContext);
            }

            await next();
        }

        private CspSendHeadersContext GetSendHeadersContext(HttpContext httpContext)
        {
            CspSendHeadersContext sendHeadersContext = null;
            if (httpContext.Items.TryGetValue(CspFilterContextKey, out var value))
            {
                sendHeadersContext = (CspSendHeadersContext)value;
            }
            return sendHeadersContext;
        }

        internal class CspSendHeadersContext
        {
            public ICspHeaderBuilder HeaderBuilder { get; set; }
            public IActivePoliciesProvider PolicyProvider { get; set; }
        }
    }
}
