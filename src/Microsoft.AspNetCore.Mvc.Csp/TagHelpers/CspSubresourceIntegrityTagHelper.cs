// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Csp.Internal;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace Microsoft.AspNetCore.Mvc.Csp.TagHelpers
{
    [HtmlTargetElement(LinkTag, Attributes = CspSubresourceIntegrityAttributeName + ", " + FallbackHrefAttributeName)]
    [HtmlTargetElement(ScriptTag, Attributes = CspSubresourceIntegrityAttributeName + ", " + FallbackSrcAttributeName)]
    public class CspSubresourceIntegrityTagHelper : UrlResolutionTagHelper
    {
        private const string LinkTag = "link";
        private const string ScriptTag = "script";
        private const string CrossOriginAttributeName = "crossorigin";
        private const string IntegrityAttributeName = "integrity";

        private const string FallbackSrcAttributeName = "asp-fallback-src";
        private const string FallbackHrefAttributeName = "asp-fallback-href";

        private const string CspSubresourceIntegrityAttributeName = "asp-subresource-integrity";
        private const string CspIntegrityAlgorithmsAttributeName = "asp-subresource-integrity-algorithms";

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
        /// <param name="htmlEncoder">The <see cref="HtmlEncoder"/>.</param>
        /// <param name="urlHelperFactory">The <see cref="IUrlHelperFactory"/>.</param>
        public CspSubresourceIntegrityTagHelper(
            IActivePoliciesProvider policyProvider, 
            IHostingEnvironment hostingEnvironment,
            IMemoryCache cache,
            HtmlEncoder htmlEncoder,
            IUrlHelperFactory urlHelperFactory)
            : base(urlHelperFactory, htmlEncoder)
        {
            _policyProvider = policyProvider;
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
        }

        // This tag helper must run before the ScriptTagHelper as it generates hashes based on the script tags markup.
        public override int Order => -1010;

        /// <summary>
        /// Gets or sets the value which determines if the hash is computed or not.
        /// </summary>
        [HtmlAttributeName(CspSubresourceIntegrityAttributeName)]
        public bool CspSubresourceIntegrity { get; set; }

        /// <summary>
        /// Gets or sets the value which determines if the hash is computed or not.
        /// </summary>
        [HtmlAttributeName(CspIntegrityAlgorithmsAttributeName)]
        public HashAlgorithms? CspIntegrityAlgorithms { get; set; }

        /// <summary>
        /// The URL of a CSS stylesheet to fallback to in the case the primary one fails.
        /// </summary>
        [HtmlAttributeName(FallbackHrefAttributeName)]
        public string FallbackHref { get; set; }

        /// <summary>
        /// The URL of a Script tag to fallback to in the case the primary one fails.
        /// </summary>
        [HtmlAttributeName(FallbackSrcAttributeName)]
        public string FallbackSrc { get; set; }

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

            if(!CspSubresourceIntegrity)
            {
                return;
            }

            // Get fallback file to be hashed
            var path = FallbackSrc ?? FallbackHref;
            if(path == null)
            {
                // User specified subresource-integrity but not a fallback; can't generate the sri hash without a local file.
                throw new InvalidOperationException(); //(Resources.CspSubresourceIntegrityTagHelper_CannotGenerateHash());
            }

            EnsureHashProvider();

            if (TryResolveUrl(path, resolvedUrl: out string resolvedUrl))
            {
                path = resolvedUrl;
            }

            // Get the provided hash algorithms or use the policy's defaults.
            var policy = await _policyProvider.GetActiveMainPolicyAsync(ViewContext.HttpContext);
            if (policy == null)
            {
                // No active policy for this request. CSP may be disabled.
                return;
            }

            var hashAlgorithms = CspIntegrityAlgorithms ?? policy.DefaultHashAlgorithms;

            var hashes = await _hashProvider.GetFileHashesAsync(path, hashAlgorithms);

            // Add the SRI attributes to the tag
            var spaceDelimitedHashes = string.Join(" ", hashes);
            output.Attributes.SetAttribute(IntegrityAttributeName, new HtmlString(spaceDelimitedHashes));
            output.Attributes.SetAttribute(CrossOriginAttributeName, "anonymous");
            
            // Update content security policy with SRI hashes.
            // This enables support for external script hashing in CSP 3.0.
            // https://www.w3.org/TR/CSP3/#external-hash    
            var directiveName = context.TagName == LinkTag ? CspDirectiveNames.StyleSrc : CspDirectiveNames.ScriptSrc;
            var scriptDirective = policy.GetOrAddDirective(directiveName);
            scriptDirective.AppendQuoted(hashes.ToArray());
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