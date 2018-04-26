// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Csp;
using Microsoft.AspNetCore.Mvc.Csp.Internal;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcCspMvcCoreBuilderExtensions
    {
        public static IMvcCoreBuilder AddCsp(this IMvcCoreBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            AddCspServices(builder.Services);
            return builder;
        }

        public static IMvcCoreBuilder AddCsp(
            this IMvcCoreBuilder builder,
            Action<CspOptions> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            AddCspServices(builder.Services);
            builder.Services.Configure(setupAction);
            
            return builder;
        }

        public static IMvcCoreBuilder ConfigureCsp(
            this IMvcCoreBuilder builder,
            Action<CspOptions> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            builder.Services.Configure(setupAction);
            return builder;
        }

        // Internal for testing.
        internal static void AddCspServices(IServiceCollection services)
        {
            services.AddCsp();

            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApplicationModelProvider, CspApplicationModelProvider>());
            services.TryAddTransient<IActivePoliciesProvider, DefaultActivePoliciesProvider>();
            services.TryAddTransient<CspActionFilter, CspActionFilter>();
        }
    }
}
