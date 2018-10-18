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
    [HtmlTargetElement(EmbedTag, Attributes = TypeAttributeName)]
    [HtmlTargetElement(ObjectTag, Attributes = TypeAttributeName)]
    public class CspPluginTypeTagHelper : TagHelper
    {
        private const string ObjectTag = "object";
        private const string EmbedTag = "embed";
        private const string TypeAttributeName = "type";

        private readonly IActivePoliciesProvider _policyProvider;

        /// <summary>
        /// Creates a new <see cref="CspPluginTypeTagHelper"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="IActivePoliciesProvider"/>.</param>
        public CspPluginTypeTagHelper(IActivePoliciesProvider policyProvider)
        {
            _policyProvider = policyProvider;
        }

        /// <summary>
        /// The plugin type to be allowed by content security policy.
        /// </summary>
        [HtmlAttributeName(TypeAttributeName)]
        public string Type { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var policy = await _policyProvider.GetActiveMainPolicyAsync(ViewContext.HttpContext);
            if (policy == null)
            {
                // No active policy for this request. CSP may be disabled.
                return;
            }

            var pluginTypes = policy.GetOrAddDirective(CspDirectiveNames.PluginTypes);
            pluginTypes.Append(Type);

            output.Attributes.SetAttribute(TypeAttributeName, Type);
        }
    }
}