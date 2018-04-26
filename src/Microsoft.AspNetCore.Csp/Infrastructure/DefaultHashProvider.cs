// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <inheritdoc />
    public class DefaultHashProvider : IHashProvider
    {
        private readonly IFileProvider _fileProvider;
        private readonly IMemoryCache _cache;
        private readonly PathString _requestPathBase;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultHashProvider"/>.
        /// </summary>
        /// <param name="fileProvider">The file provider to get and watch files.</param>
        /// <param name="cache"><see cref="IMemoryCache"/> where the file and inline script hashes are cached.</param>
        /// <param name="requestPathBase">The base path for the current HTTP request.</param>
        public DefaultHashProvider(
            IFileProvider fileProvider,
            IMemoryCache cache,
            PathString requestPathBase)
        {
            if (fileProvider == null)
            {
                throw new ArgumentNullException(nameof(fileProvider));
            }

            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            _fileProvider = fileProvider;
            _cache = cache;
            _requestPathBase = requestPathBase;
        }

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetContentHashesAsync(string cacheKey, string content, HashAlgorithms hashAlgorithms)
        {
            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<string> hashes))
            {
                hashes = GetHashesForString(content, hashAlgorithms);
                hashes = _cache.Set(cacheKey, hashes);
            }

            return Task.FromResult(hashes);
        }

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetFileHashesAsync(string path, HashAlgorithms hashAlgorithms)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!_cache.TryGetValue(path, out IEnumerable<string> hashes))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions();
                cacheEntryOptions.AddExpirationToken(_fileProvider.Watch(path));
                var fileInfo = _fileProvider.GetFileInfo(path);

                if (!fileInfo.Exists &&
                    _requestPathBase.HasValue &&
                    path.StartsWith(_requestPathBase.Value, StringComparison.OrdinalIgnoreCase))
                {
                    var requestPathBaseRelativePath = path.Substring(_requestPathBase.Value.Length);
                    cacheEntryOptions.AddExpirationToken(_fileProvider.Watch(requestPathBaseRelativePath));
                    fileInfo = _fileProvider.GetFileInfo(requestPathBaseRelativePath);
                }

                if (fileInfo.Exists)
                {
                    hashes = GetHashesForFile(fileInfo, hashAlgorithms);
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