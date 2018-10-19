// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Csp.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace Microsoft.AspNetCore.Mvc.Csp.TagHelpers
{
    [HtmlTargetElement(ScriptTag, Attributes = CspGenerateHashAttributeName)]
    [HtmlTargetElement(StyleTag, Attributes = CspGenerateHashAttributeName)]
    public class CspGenerateHashTagHelper : TagHelper
    {
        private const string ScriptTag = "script";
        private const string StyleTag = "style";
        private const string CspGenerateHashAttributeName = "asp-generate-hash";
        private const string CspGenerateHashAlgorithmsAttributeName = "asp-generate-hash-algorithms";
        private readonly IActivePoliciesProvider _policyProvider;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMemoryCache _cache;
        private IHashProvider _hashProvider;

        /// <summary>
        /// Creates a new <see cref="CspPluginTypeTagHelper"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="IActivePoliciesProvider"/>.</param>
        /// <param name="hostingEnvironment">The <see cref="IHostingEnvironment"/>.</param>
        /// <param name="cache">The <see cref="IMemoryCache"/>.</param>
        public CspGenerateHashTagHelper(
            IActivePoliciesProvider policyProvider,
            IHostingEnvironment hostingEnvironment,
            IMemoryCache cache)
        {
            _policyProvider = policyProvider;
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
        }

        /// <summary>
        /// Gets or sets the value which determines if the hash is computed or not.
        /// </summary>
        [HtmlAttributeName(CspGenerateHashAttributeName)]
        public bool CspGenerateHash { get; set; }

        /// <summary>
        /// Gets or sets the value which determines if the hash is computed or not.
        /// </summary>
        [HtmlAttributeName(CspGenerateHashAlgorithmsAttributeName)]
        public HashAlgorithms? CspHashAlgorithms { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <inheritdoc />
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if(!CspGenerateHash)
            {
                return;
            }

            EnsureHashProvider();

            // Get the child content that is to be hashed.
            var content = await output.GetChildContentAsync(); 

            // Get the provided hash algorithms or use the policy's defaults.
            var policy = await _policyProvider.GetActiveMainPolicyAsync(ViewContext.HttpContext);
            if (policy == null)
            {
                // No active policy for this request. CSP may be disabled.
                return;
            }

            var hashAlgorithms = CspHashAlgorithms ?? policy.DefaultHashAlgorithms;

			// TODO: Investigate why this newline replacement is necessary.
	        var contentToHash = content.GetContent().Replace("\r\n", "\n");

			// Hash the content or use cached values.
			var hashes = await _hashProvider.GetContentHashesAsync(context.UniqueId, contentToHash, hashAlgorithms);

            // Update the main policy with the inline script/style hashes.
            var directiveName = output.TagName == StyleTag ? CspDirectiveNames.StyleSrc : CspDirectiveNames.ScriptSrc;
            var directive = policy.GetOrAddDirective(directiveName);
            directive.AppendQuoted(hashes.ToArray());
        }

        /// <summary>
        /// Ensures the hash provider is instantiated since injecting it would introduce
        /// a dependency on IHttpContextAccessor which has non-trivial performance impacts.
        /// https://github.com/aspnet/Hosting/issues/793
        /// </summary>
        private void EnsureHashProvider()
        {
            if (_hashProvider == null)
            {
                _hashProvider = new DefaultHashProvider(
                    _hostingEnvironment.WebRootFileProvider,
                    _cache,
                    ViewContext.HttpContext.Request.PathBase);
            }
        }
    }
}