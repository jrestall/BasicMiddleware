// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// A type which can provide hashes for given content or files.
    /// </summary>
    public interface IHashProvider
    {
        /// <summary>
        /// Provides the computed hashes of a given content string.
        /// </summary>
        /// <param name="cacheKey">The key used to cache the result of the hash.</param>
        /// <param name="content">The content to be hashed.</param>
        /// <param name="hashAlgorithms">The hash algorithms used to hash the content.</param>
        /// <returns>A collection of hashes, one for each hash algorithm specified.</returns>
        /// <remarks>Each computed hash is prefixed with the algorithm e.g. sha512-{hash} for the SHA512 algorithm.</remarks>
        Task<IList<string>> GetContentHashesAsync(string cacheKey, string content, HashAlgorithms hashAlgorithms);

        /// <summary>
        /// Provides the computed hashes of a given local file.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> associated with the request.</param>
        /// <param name="path">The path to the local file to be hashed.</param>
        /// <param name="hashAlgorithms">The hash algorithms used to hash the content.</param>
        /// <returns>A collection of hashes, one for each hash algorithm specified.</returns>
        /// <remarks>Each computed hash is prefixed with the algorithm e.g. sha512-{hash} for the SHA512 algorithm.</remarks>
        Task<IList<string>> GetFileHashesAsync(HttpContext httpContext, string path, HashAlgorithms hashAlgorithms);
    }
}