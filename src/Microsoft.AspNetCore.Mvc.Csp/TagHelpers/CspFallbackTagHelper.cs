// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
    [HtmlTargetElement(LinkTag, Attributes = "asp-fallback-href")]
    [HtmlTargetElement(ScriptTag, Attributes = "asp-fallback-src")]
    public class CspFallBackTagHelper : TagHelper
    {
        private const string LinkTag = "link";
        private const string ScriptTag = "script";

        private readonly IActivePoliciesProvider _policyProvider;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMemoryCache _cache;
        private IHashProvider _hashProvider;

        /// <summary>
        /// Creates a new <see cref="CspFallBackTagHelper"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="IActivePoliciesProvider"/>.</param>
        /// <param name="hostingEnvironment">The <see cref="IHostingEnvironment"/>.</param>
        /// <param name="cache">The <see cref="IMemoryCache"/>.</param>
        public CspFallBackTagHelper(
            IActivePoliciesProvider policyProvider, 
            IHostingEnvironment hostingEnvironment,
            IMemoryCache cache)
        {
            _policyProvider = policyProvider;
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
        }

        // This tag helper must run after the ScriptTagHelper as it generates hashes based on the script tags markup.
        public override int Order => -1000 + 100;

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

            // Check if the ScriptTagHelper has actually output the fallback <script> tags.
            if (output.PostElement.IsEmptyOrWhiteSpace)
            {
                return;
            }

            // Check that there is an active content security policy that would require inline scripts to be hashed. 
            var policy = await _policyProvider.GetActiveMainPolicyAsync(ViewContext.HttpContext);
            if (policy == null)
            {
                // No active policy for this request. CSP may be disabled.
                return;
            }

            EnsureHashProvider();

            // Search for fallback script, hash it and add the hash to the content security policy.
            // Example PostContent output:
            //  <script src="//ajax.aspnetcdn.com/ajax/bootstrap/3.0.0/bootstrap.min.js"></script>
            //  <script>(typeof($.fn.modal) === 'undefined'||document.write("<script src=\"\/lib\/bootstrap\/js\/bootstrap.min.js\"><\/script>"));</script>

            var postContent = output.PostElement.GetContent();
            var fallbackScriptHashes = await GetFallbackScriptHash(context.UniqueId, postContent);
            if (fallbackScriptHashes == null)
            {
                // TODO: Exception?
                return;
            }

            var scriptDirective = policy.GetOrAddDirective(CspDirectiveNames.ScriptSrc);
            scriptDirective.AppendQuoted(fallbackScriptHashes.ToArray());
        }

        private async Task<IEnumerable<string>> GetFallbackScriptHash(string cacheKey, string postContent)
        {
            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (postContent == null)
            {
                throw new ArgumentNullException(nameof(postContent));
            }

            var inlineScript = FindInlineScriptTag(postContent);
            if(inlineScript == null)
            {         
                return null;
            }

            return await _hashProvider.GetContentHashesAsync(cacheKey, inlineScript, HashAlgorithms.SHA256);
        }

        private string FindInlineScriptTag(string htmlContent)
        {
            int from = htmlContent.LastIndexOf("<script>", StringComparison.Ordinal) + "<script>".Length;
            if(from == -1) return null;

            int to = htmlContent.LastIndexOf("</script>", StringComparison.Ordinal);
            if(to == -1) return null;

            return htmlContent.Substring(from, to - from);
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