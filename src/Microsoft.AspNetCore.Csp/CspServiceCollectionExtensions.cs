// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up content security policy services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class CspServiceCollectionExtensions
    {
        /// <summary>
        /// Adds content security policy services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddCsp(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();

            services.TryAddTransient<INonceProvider, DefaultNonceProvider>();
            services.TryAddTransient<IHashProvider, DefaultHashProvider>();
            services.TryAddTransient<ICspProvider, DefaultCspProvider>();
            services.TryAddTransient<ICspHeaderBuilder, CspHeaderBuilder>();

            return services;
        }

        /// <summary>
        /// Adds content security policy services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="setupAction">An <see cref="Action{CspOptions}"/> to configure the provided <see cref="CspOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddCsp(this IServiceCollection services, Action<CspOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddCsp();
            services.Configure(setupAction);

            return services;
        }
    }
}