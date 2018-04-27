// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// A type which can provide a nonce for a particular <see cref="HttpContext"/>.
    /// </summary>
    public interface INonceProvider
    {
        /// <summary>
        /// Gets a base64 encoded nonce for the curent request.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> associated with the request.</param>
        /// <returns>The base64 encoded nonce for the current request.</returns>
        string GetNonce(HttpContext httpContext);
    }
}