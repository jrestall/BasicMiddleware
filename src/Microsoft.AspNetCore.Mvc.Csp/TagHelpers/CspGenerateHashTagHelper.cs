// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Mvc.Csp.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

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
        private readonly IHashProvider _hashProvider;

        /// <summary>
        /// Creates a new <see cref="CspPluginTypeTagHelper"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="IActivePoliciesProvider"/>.</param>
        /// <param name="hashProvider">The <see cref="IHashProvider"/>.</param>
        public CspGenerateHashTagHelper(
            IActivePoliciesProvider policyProvider,
            IHashProvider hashProvider)
        {
            _policyProvider = policyProvider;
            _hashProvider = hashProvider;
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

            // Get the child content that is to be hashed.
            var content = await output.GetChildContentAsync(); 

            // Get the provided hash algorithms or use the policy's defaults.
            var policy = await _policyProvider.GetActiveMainPolicyAsync(ViewContext.HttpContext);
            var hashAlgorithms = CspHashAlgorithms ?? policy.DefaultHashAlgorithms;

            // Hash the content or use cached values.
            var hashes = await _hashProvider.GetContentHashesAsync(context.UniqueId, content.GetContent(), hashAlgorithms);

            // Update the main policy with the inline script/style hashes.
            var directiveName = output.TagName == StyleTag ? CspDirectiveNames.StyleSrc : CspDirectiveNames.ScriptSrc;
            var directive = policy.GetOrAddDirective(directiveName);
            directive.AppendQuoted(hashes.ToArray());
        }
    }
}