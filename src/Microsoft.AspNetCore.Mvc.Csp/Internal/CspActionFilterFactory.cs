// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.Csp.Internal
{
    /// <summary>
    /// A filter factory which creates a new instance of <see cref="CspActionFilter"/>.
    /// </summary>
    public class CspActionFilterFactory : IFilterFactory, IOrderedFilter
    {
        private readonly string[] _policyNames;

        /// <summary>
        /// Creates a new instance of <see cref="CspActionFilterFactory"/>.
        /// </summary>
        /// <param name="policyNames">Names used to fetch the content security policies.</param>
        public CspActionFilterFactory(params string[] policyNames)
        {
            _policyNames = policyNames;
        }

        /// <inheritdoc />
        // Since this filter sets the content security policy headers that should be present
        // on every response, this filter must run before any other authorization filters.
        public int Order => int.MinValue + 80;

        /// <inheritdoc />
        public bool IsReusable => true;

        /// <inheritdoc />
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var filter = serviceProvider.GetRequiredService<CspActionFilter>();
            filter.PolicyNames = _policyNames;

            return filter;
        }
    }
}
