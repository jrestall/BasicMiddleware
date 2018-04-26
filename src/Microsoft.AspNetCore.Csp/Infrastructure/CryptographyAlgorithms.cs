// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Cryptography;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// The cryptography algorithms that are used for generating hashes in a content security policy.
    /// </summary>
    public static class CryptographyAlgorithms
    {
        public static SHA256 CreateSHA256()
        {
            try
            {
                return SHA256.Create();
            }
            // SHA256.Create is documented to throw this exception on FIPS compliant machines.
            // See: https://msdn.microsoft.com/en-us/library/z08hz7ad(v=vs.110).aspx
            catch (System.Reflection.TargetInvocationException)
            {
                // Fallback to a FIPS compliant SHA256 algorithm.
                return new SHA256CryptoServiceProvider();
            }
        }

        public static SHA384 CreateSHA384()
        {
            try
            {
                return SHA384.Create();
            }
            // SHA384.Create is documented to throw this exception on FIPS compliant machines.
            // See: https://msdn.microsoft.com/en-us/library/ww69k51z(v=vs.110).aspx
            catch (System.Reflection.TargetInvocationException)
            {
                // Fallback to a FIPS compliant SHA384 algorithm.
                return new SHA384CryptoServiceProvider();
            }
        }

        public static SHA512 CreateSHA512()
        {
            try
            {
                return SHA512.Create();
            }
            // SHA512.Create is documented to throw this exception on FIPS compliant machines.
            // See: https://msdn.microsoft.com/en-us/library/hydyw22a(v=vs.110).aspx
            catch (System.Reflection.TargetInvocationException)
            {
                // Fallback to a FIPS compliant SHA512 algorithm.
                return new SHA512CryptoServiceProvider();
            }
        }
    }
}