// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Csp.Infrastructure;

namespace Microsoft.AspNetCore.Csp
{
    /// <inheritdoc cref="IDisableCspAttribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DisableCspAttribute : Attribute, IDisableCspAttribute
    {
    }
}