// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <inheritdoc />
    public class DefaultNonceProvider : INonceProvider
    {
        /// <summary>
        /// The length in bits of the randomly generated nonce. Default is 128 bits.
        /// </summary>
        public static int DefaultNonceBitLength { get; set; } = 128;

        /// <inheritdoc />
        public string GetNonce(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            // Check cache as it's important we only generate one nonce per request.
            string nonce;
            if (httpContext.Items.TryGetValue(nameof(DefaultNonceProvider), out var nonceObject) &&
                (nonce = (string)nonceObject) != null)
            {
                return nonce;
            }

            nonce = CreateNonce();

            httpContext.Items[nameof(DefaultNonceProvider)] = nonce;

            return nonce;
        }

        protected virtual string CreateNonce()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var nonceBytes = new byte[DefaultNonceBitLength / 8];
                rng.GetBytes(nonceBytes);
                return Convert.ToBase64String(nonceBytes);
            }
        }
    }
}