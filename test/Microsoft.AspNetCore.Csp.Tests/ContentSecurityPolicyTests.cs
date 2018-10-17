// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Csp.Infrastructure;
using Xunit;

namespace Microsoft.AspNetCore.Csp.Tests
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
        public void CanAppendToSourceList()
        {
			// Arrange
	        // Produces: default-src 'self'; script-src 'none';
			var policy = new ContentSecurityPolicyBuilder()
		        .AddDefaultSrc(src => src.AllowSelf())
		        .AddScriptSrc(src => src.AllowNone())
		        .Build();

			// Act
			// Append value to the source list, override 'none' values
			// Produces: default-src 'self'; script-src 'self' example.org; object-src 'self' dot.net
			policy.Append(p =>
			{
				p.AddScriptSrc(src => src.AllowHost("example.org"));
				p.AddObjectSrc(src => src.AllowHost("dot.net"));
			});

            // Assert
            Assert.Equal(3, policy.Directives.Count);
	        Assert.Equal("'self'", policy.Directives["default-src"].Value);
			Assert.Equal("'self' example.org", policy.Directives["script-src"].Value);
			Assert.Equal("dot.net", policy.Directives["object-src"].Value);
		}

	    [Fact]
	    public void CanOverridePreviouslySetSourceList()
	    {
			// Arrange
			// Produces: default-src 'self'; script-src 'none';
			var policy = new ContentSecurityPolicyBuilder()
			    .AddDefaultSrc(src => src.AllowSelf())
			    .AddScriptSrc(src => src.AllowNone())
			    .Build();

			// Act
			// Overrides the previously set source list, override 'none' values
			// Produces: default-src 'self'; script-src example.org; object-src 'self'
			policy.Override(p =>
		    {
			    p.AddScriptSrc(src => src.AllowHost("example.org"));
			    p.AddObjectSrc(src => src.AllowSelf());
		    });

		    // Assert
		    Assert.Equal(3, policy.Directives.Count);
		    Assert.Equal("'self'", policy.Directives["default-src"].Value);
		    Assert.Equal("example.org", policy.Directives["script-src"].Value);
		    Assert.Equal("'self'", policy.Directives["object-src"].Value);
	    }
	}
}