// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    public class ContentSecurityPolicyTest
    {
        [Fact]
        public void Default_Constructor()
        {
            // Arrange & Act
            var policy = new ContentSecurityPolicy();

            // Assert
            Assert.False(policy.ReportOnly);
            Assert.Empty(policy.Directives);
        }

        [Fact]
        public void AppendingOverridesNoneValues()
        {
            // Arrange
            var policy = new ContentSecurityPolicy();
            policy.AddDirective(CspDirectiveNames.ScriptSrc, src => src.AllowNone());

            // Act
            policy.AppendDirective(CspDirectiveNames.ScriptSrc, src => src.AllowSelf());

            // Assert
            Assert.Equal(1, policy.Directives.Count());
            Assert.Equal("'self'", policy.Directives["script-src"].Value);
        }

        [Fact]
        public void ToString_ReturnsThePropertyValues()
        {
            // Arrange
            var corsPolicy = new CorsPolicy
            {
                PreflightMaxAge = TimeSpan.FromSeconds(12),
                SupportsCredentials = true
            };
            corsPolicy.Headers.Add("foo");
            corsPolicy.Headers.Add("bar");
            corsPolicy.Origins.Add("http://example.com");
            corsPolicy.Origins.Add("http://example.org");
            corsPolicy.Methods.Add("GET");

            // Act
            var policyString = corsPolicy.ToString();

            // Assert
            Assert.Equal(
                @"AllowAnyHeader: False, AllowAnyMethod: False, AllowAnyOrigin: False, PreflightMaxAge: 12,"+
                " SupportsCredentials: True, Origins: {http://example.com,http://example.org}, Methods: {GET},"+
                " Headers: {foo,bar}, ExposedHeaders: {}",
                policyString);
        }
    }
}