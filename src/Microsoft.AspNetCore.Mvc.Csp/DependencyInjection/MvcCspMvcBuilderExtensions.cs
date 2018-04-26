// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up content security policy services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class MvcCspMvcBuilderExtensions
    {
        /// <summary>
        /// Adds the 'application/csp-report' media type to the input formatters so that csp reports can be received.
        /// </summary>
        /// <param name="builder">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IMvcBuilder AddCspReportMediaType(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.Configure<MvcOptions>(options =>
                {
                    options.InputFormatters
                        .Where(item => item.GetType() == typeof(JsonInputFormatter))
                        .Cast<JsonInputFormatter>()
                        .Single()
                        .SupportedMediaTypes
                        .Add("application/csp-report");
                }
            );

            return builder;
        }
    }
}