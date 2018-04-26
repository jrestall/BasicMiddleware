// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// The <see cref="IApplicationBuilder"/> extensions for adding content security policy middleware support.
    /// </summary>
    public static class CspMiddlewareExtensions
    {
        /// <summary>
        /// Adds a middleware to your web application pipeline to send content security policy response headers.
        /// </summary>
        /// <param name="app">The IApplicationBuilder passed to your Configure method</param>
        /// <returns>The original app parameter</returns>
        public static IApplicationBuilder UseCsp(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<CspMiddleware>();
        }

        /// <summary>
        /// Adds a middleware to your web application pipeline to send content security policy response headers.
        /// </summary>
        /// <param name="app">The IApplicationBuilder passed to your Configure method</param>
        /// <param name="policyName">The policy name of a configured policy.</param>
        /// <returns>The original app parameter</returns>
        public static IApplicationBuilder UseCsp(this IApplicationBuilder app, string policyName)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<CspMiddleware>(policyName);
        }

        /// <summary>
        /// Adds a middleware to your web application pipeline to send content security policy response headers.
        /// </summary>
        /// <param name="app">The IApplicationBuilder passed to your Configure method.</param>
        /// <param name="configurePolicy">A delegate which can use a policy builder to build a policy.</param>
        /// <returns>The original app parameter</returns>
        public static IApplicationBuilder UseCsp(
            this IApplicationBuilder app,
            Action<ContentSecurityPolicyBuilder> configurePolicy)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (configurePolicy == null)
            {
                throw new ArgumentNullException(nameof(configurePolicy));
            }

            var policyBuilder = new ContentSecurityPolicyBuilder();
            configurePolicy(policyBuilder);
            return app.UseMiddleware<CspMiddleware>(policyBuilder.Build());
        }
    }
}