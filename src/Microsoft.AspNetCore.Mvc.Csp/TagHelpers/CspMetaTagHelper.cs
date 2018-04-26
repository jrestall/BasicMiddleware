// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Mvc.Csp.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Microsoft.AspNetCore.Mvc.Csp.TagHelpers
{
    [HtmlTargetElement("meta", 
        Attributes = "[http-equiv=Content-Security-Policy]", 
        ParentTag = "head", 
        TagStructure = TagStructure.WithoutEndTag)]
    public class CspMetaTagHelper : TagHelper
    {
        private const string ContentAttributeName = "content";
        private readonly IActivePoliciesProvider _policyProvider;
        private readonly ICspHeaderBuilder _cspHeaderBuilder;

        /// <summary>
        /// Creates a new <see cref="CspPluginTypeTagHelper"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="IActivePoliciesProvider"/>.</param>
        /// <param name="cspHeaderBuilder">The <see cref="ICspHeaderBuilder"/>.</param>
        public CspMetaTagHelper(IActivePoliciesProvider policyProvider, ICspHeaderBuilder cspHeaderBuilder)
        {
            _policyProvider = policyProvider;
            _cspHeaderBuilder = cspHeaderBuilder;
        }

        /// <summary>
        /// The meta tag content attribute.
        /// </summary>
        [HtmlAttributeName(ContentAttributeName)]
        public string Content { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Retrieve the TagHelperOutput variation of the "content" attribute
            // in case other TagHelpers in the pipeline have touched the value.
            Content = output.Attributes[ContentAttributeName]?.Value as string;

            if(!string.IsNullOrEmpty(Content))
            {
                // A content attribute value is already set for the Content-Security-Policy
                // meta tag. Don't override the existing policy.
                return;
            }

            var policy = await _policyProvider.GetActiveMainPolicyAsync(ViewContext.HttpContext);
            //TODO: Support outputting multiple meta tags.
            //foreach (var policy in policies)
            //{
                var header = _cspHeaderBuilder.GetHeader(ViewContext.HttpContext, policy, supportMetaTag: true);

                output.Attributes.SetAttribute("http-equiv", header.Name);
                output.Attributes.SetAttribute(ContentAttributeName, header.Value);
            //}     
        }
    }
}