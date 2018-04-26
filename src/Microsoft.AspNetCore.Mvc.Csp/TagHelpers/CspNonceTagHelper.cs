// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Mvc.Csp.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Microsoft.AspNetCore.Mvc.Csp.TagHelpers
{
    [HtmlTargetElement(ScriptTag, Attributes = CspIncludeNonceAttributeName)]
    [HtmlTargetElement(StyleTag, Attributes = CspIncludeNonceAttributeName)]
    public class CspNonceTagHelper : TagHelper
    {
        private const string ScriptTag = "script";
        private const string StyleTag = "style";
        private const string CspIncludeNonceAttributeName = "asp-add-nonce";

        private readonly IActivePoliciesProvider _policyProvider;
        private readonly INonceProvider _nonceProvider;

        /// <summary>
        /// Creates a new <see cref="CspNonceTagHelper"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="IActivePoliciesProvider"/>.</param>
        /// <param name="nonceProvider">The <see cref="INonceProvider"/>.</param>
        public CspNonceTagHelper(IActivePoliciesProvider policyProvider, INonceProvider nonceProvider)
        {
            _policyProvider = policyProvider;
            _nonceProvider = nonceProvider;
        }

        /// <summary>
        /// Gets or sets the value which determines if the nonce is output or not.
        /// </summary>
        [HtmlAttributeName(CspIncludeNonceAttributeName)]
        public bool CspIncludeNonce { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if(!CspIncludeNonce)
            {
                return;
            }

            var policy = await _policyProvider.GetActiveMainPolicyAsync(ViewContext.HttpContext);
            var directiveName = context.TagName == StyleTag ? CspDirectiveNames.StyleSrc : CspDirectiveNames.ScriptSrc;

            // Ensure the directive is configured to return a nonce.
            var directive = policy.GetOrAddDirective(directiveName);
            directive.AddNonce = true;

            var nonce = _nonceProvider.GetNonce(ViewContext.HttpContext);
            output.Attributes.SetAttribute("nonce", nonce);
        }
    }
}