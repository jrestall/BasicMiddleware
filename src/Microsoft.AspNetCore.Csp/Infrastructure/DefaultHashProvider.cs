// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <inheritdoc />
    public class DefaultHashProvider : IHashProvider
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultHashProvider"/>.
        /// </summary>
        /// <param name="hostingEnvironment">The current hosting environment.</param>
        /// <param name="cache"><see cref="IMemoryCache"/> where the file and inline script hashes are cached.</param>
        public DefaultHashProvider(
            IHostingEnvironment hostingEnvironment,
            IMemoryCache cache)
        {
            if (hostingEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostingEnvironment));
            }

            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
        }

        /// <inheritdoc />
        public Task<IList<string>> GetContentHashesAsync(string cacheKey, string content, HashAlgorithms hashAlgorithms)
        {
            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (!_cache.TryGetValue(cacheKey, out IList<string> hashes))
            {
                hashes = GetHashesForString(content, hashAlgorithms).ToList();
                hashes = _cache.Set(cacheKey, hashes);
            }

            return Task.FromResult(hashes);
        }

        /// <inheritdoc />
        public Task<IList<string>> GetFileHashesAsync(HttpContext httpContext, string path, HashAlgorithms hashAlgorithms)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!_cache.TryGetValue(path, out IList<string> hashes))
            {
                var fileProvider = _hostingEnvironment.WebRootFileProvider;

                var cacheEntryOptions = new MemoryCacheEntryOptions();
                cacheEntryOptions.AddExpirationToken(fileProvider.Watch(path));
                var fileInfo = fileProvider.GetFileInfo(path);

                var requestPathBase = httpContext.Request.PathBase;
                if (!fileInfo.Exists &&
                    requestPathBase.HasValue &&
                    path.StartsWith(requestPathBase.Value, StringComparison.OrdinalIgnoreCase))
                {
                    var requestPathBaseRelativePath = path.Substring(requestPathBase.Value.Length);
                    cacheEntryOptions.AddExpirationToken(fileProvider.Watch(requestPathBaseRelativePath));
                    fileInfo = fileProvider.GetFileInfo(requestPathBaseRelativePath);
                }

                if (fileInfo.Exists)
                {
                    hashes = GetHashesForFile(fileInfo, hashAlgorithms).ToList();
                }
                else
                {
                    // if the file is not in the current server.
                    // TODO: throw
                    return null;
                }

                hashes = _cache.Set(path, hashes, cacheEntryOptions);
            }

            return Task.FromResult(hashes);
        }

        private static IEnumerable<string> GetHashesForString(string content, HashAlgorithms hashAlgorithms)
        {
            foreach (var hashAlgorithm in GetFlags(hashAlgorithms))
            {
                yield return GetHashForString(content, hashAlgorithm);
            }
        }

        private static IEnumerable<string> GetHashesForFile(IFileInfo fileInfo, HashAlgorithms hashAlgorithms)
        {
            foreach (var hashAlgorithm in GetFlags(hashAlgorithms))
            {
                yield return GetHashForFile(fileInfo, hashAlgorithm);
            }
        }

        private static string GetHashForFile(IFileInfo fileInfo, HashAlgorithms hashAlgorithm)
        {
            using (var algorithm = CreateHashAlgorithm(hashAlgorithm))
            {
                using (var readStream = fileInfo.CreateReadStream())
                {
                    var hashPrefix = GetHashPrefix(hashAlgorithm);
                    var hash = algorithm.ComputeHash(readStream);
                    return hashPrefix + Convert.ToBase64String(hash);   
                }
            }
        }

        private static string GetHashForString(string content, HashAlgorithms hashAlgorithm)
        {
            using (var algorithm = CreateHashAlgorithm(hashAlgorithm))
            {       
                var hashPrefix = GetHashPrefix(hashAlgorithm);
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(content));
                return hashPrefix + Convert.ToBase64String(hash);      
            }
        }

        private static HashAlgorithm CreateHashAlgorithm(HashAlgorithms hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
                case HashAlgorithms.SHA256:
                    return CryptographyAlgorithms.CreateSHA256();
                case HashAlgorithms.SHA384:
                    return CryptographyAlgorithms.CreateSHA384();
                case HashAlgorithms.SHA512:
                    return CryptographyAlgorithms.CreateSHA512();
                default:
                    throw new ArgumentOutOfRangeException($"Hash algorithm {hashAlgorithm} is not recognized.", nameof(hashAlgorithm));
            }
        }

        private static string GetHashPrefix(HashAlgorithms hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
                case HashAlgorithms.SHA256:
                    return "sha256-";
                case HashAlgorithms.SHA384:
                    return "sha384-";
                case HashAlgorithms.SHA512:
                    return "sha512-";
                default:
                    throw new ArgumentOutOfRangeException($"Hash algorithm {hashAlgorithm} is not recognized.", nameof(hashAlgorithm));
            }
        }

        private static IEnumerable<HashAlgorithms> GetFlags(Enum enumeration)
        {
            foreach (HashAlgorithms value in Enum.GetValues(enumeration.GetType()))
            {
                if (enumeration.HasFlag(value))
                {
                    yield return value;
                }
            }
        }
    }
}