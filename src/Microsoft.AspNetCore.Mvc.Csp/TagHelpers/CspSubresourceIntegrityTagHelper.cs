// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Csp.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Microsoft.AspNetCore.Mvc.Csp.TagHelpers
{
    [HtmlTargetElement(LinkTag, Attributes = CspSubresourceIntegrityAttributeName + ", " + FallbackHrefAttributeName)]
    [HtmlTargetElement(ScriptTag, Attributes = CspSubresourceIntegrityAttributeName + ", " + FallbackSrcAttributeName)]
    public class CspSubresourceIntegrityTagHelper : TagHelper
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
        private readonly IHashProvider _hashProvider;

        /// <summary>
        /// Creates a new <see cref="CspPluginTypeTagHelper"/>.
        /// </summary>
        /// <param name="policyProvider">The <see cref="IActivePoliciesProvider"/>.</param>
        /// <param name="hashProvider">The <see cref="IHashProvider"/>.</param>
        public CspSubresourceIntegrityTagHelper(IActivePoliciesProvider policyProvider, IHashProvider hashProvider)
        {
            _policyProvider = policyProvider;
            _hashProvider = hashProvider;
        }

        // This filter must run after the ScriptTagHelper as it generates hashes based on the script tags markup.
        public override int Order
        {
            get  {  return -1000 + 100;   }
        }

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

            // Get the provided hash algorithms or use the policy's defaults.
            var policy = await _policyProvider.GetActiveMainPolicyAsync(ViewContext.HttpContext);
            var hashAlgorithms = CspIntegrityAlgorithms ?? policy.DefaultHashAlgorithms;

            var hashes = await _hashProvider.GetFileHashesAsync(path, hashAlgorithms);

            // Add the SRI attributes to the tag
            var spaceDelimitedHashes = string.Join(" ", hashes);
            output.Attributes.SetAttribute(IntegrityAttributeName, new HtmlString(spaceDelimitedHashes));
            output.Attributes.SetAttribute(CrossOriginAttributeName, "anonymous");
            
            // Update content security policy with SRI hashes.
            // This enables support for external script hashing in CSP 3.0.
            // https://www.w3.org/TR/CSP3/#external-hash    
            var scriptDirective = policy.GetOrAddDirective(CspDirectiveNames.ScriptSrc);
            scriptDirective.Append(spaceDelimitedHashes);
            
            if(!output.PostContent.IsModified)
            {
                return;
            }

            if(context.TagName == ScriptTag)
            {
                // Search for fallback script if added, hash it and add to CSP.
                // Example PostContent output:
                //  <script src="//ajax.aspnetcdn.com/ajax/bootstrap/3.0.0/bootstrap.min.js"></script>
                //  <script>(typeof($.fn.modal) === 'undefined'||document.write("<script src=\"\/lib\/bootstrap\/js\/bootstrap.min.js\"><\/script>"));</script>

                await SecureFallbackScripts("<script ", context, policy, output.PostContent, spaceDelimitedHashes);
            }
            else if(context.TagName == LinkTag)
            {
                // Search for fallback style if added, hash it and add to CSP.
                // Example PostContent output:
                //  <link rel="stylesheet" href="//ajax.aspnetcdn.com/ajax/bootstrap/3.0.0/css/bootstrap.min.css" />
                //  <meta name="x-stylesheet-fallback-test" class="hidden" />
                //  <script>!function(a,b,c){var d,e=document,f=e.getElementsByTagName("SCRIPT"),g=f[f.length-1].previousElementSibling,h=e.defaultView&amp;&amp;e.defaultView.getComputedStyle?e.defaultView.getComputedStyle(g):g.currentStyle;if(h&amp;&amp;h[a]!==b)for(d=0;d<c.length;d++)e.write('<link rel="stylesheet" href="'+c[d]+'"/>')}("visibility","hidden",["\/lib\/bootstrap\/css\/bootstrap.min.css"]);</script>

                await SecureFallbackScripts("<link ", context, policy, output.PostContent, spaceDelimitedHashes);
            }
        }

        private async Task SecureFallbackScripts(string startTag, TagHelperContext context, ContentSecurityPolicy policy, TagHelperContent postContent, string hashes)
        {
            var content = postContent.GetContent();
            // Insert SRI hash into fallback loading script
            var attributeInsertIndex = content.LastIndexOf(startTag, StringComparison.Ordinal);
            var modifiedPostContent = content.Insert(attributeInsertIndex + startTag.Length, $"integrity=\\\"{hashes}\\\" ");
            postContent.SetHtmlContent(modifiedPostContent);

            // Hash the fallback script and add to content security policy
            var inlineScript = FindInlineScriptTag(modifiedPostContent);
            if(inlineScript == null)
            {
                // TODO: Exception
                return;
            }

            var fallbackScriptHashes = await _hashProvider.GetContentHashesAsync(context.UniqueId, inlineScript, policy.DefaultHashAlgorithms);
            

            var scriptDirective = policy.GetOrAddDirective(CspDirectiveNames.ScriptSrc);
            scriptDirective.Append(fallbackScriptHashes.ToArray()); 
        }

        private string FindInlineScriptTag(string htmlContent)
        {
            int from = htmlContent.LastIndexOf("<script>", StringComparison.Ordinal) + "<script>".Length;
            if(from == -1) return null;

            int to = htmlContent.LastIndexOf("</script>", StringComparison.Ordinal);
            if(to == -1) return null;

            return htmlContent.Substring(from, to - from);
        }
    }
}