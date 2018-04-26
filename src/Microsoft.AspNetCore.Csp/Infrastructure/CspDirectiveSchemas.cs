// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// Schemas that can be used to allow certain URIs as content sources.
    /// </summary>
    public static class CspDirectiveSchemas
    {
        /// <summary>
        /// Allows http: URIs to be used as a content source.
        /// </summary>
        public const string Http = "http:";

        /// <summary>
        /// Allows https: URIs to be used as a content source.
        /// </summary>
        public const string Https = "https:";

        /// <summary>
        /// Allows data: URIs to be used as a content source.
        /// </summary>
        /// <remarks>This is insecure; an attacker can also inject arbitrary data: URIs. Use this sparingly and definitely not for scripts.</remarks>
        public const string Data = "data:";

        /// <summary>
        /// Allows mediastream: URIs to be used as a content source.
        /// </summary>
        public const string MediaStream = "mediastream:";

        /// <summary>
        /// Allows blob: URIs to be used as a content source.
        /// </summary>
        public const string Blob = "blob:";

        /// <summary>
        /// Allows filesystem: URIs to be used as a content source.
        /// </summary>
        public const string FileSystem = "filesystem:";
    }
}
