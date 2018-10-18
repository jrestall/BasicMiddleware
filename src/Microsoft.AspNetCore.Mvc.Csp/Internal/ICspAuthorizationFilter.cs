// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.Csp.Internal
{
    /// <summary>
    /// A filter that can be used to enable/disable content security policy support for a resource.
    /// </summary>
    public interface ICspAuthorizationFilter : IAsyncAuthorizationFilter, IOrderedFilter
    {
    }
}
