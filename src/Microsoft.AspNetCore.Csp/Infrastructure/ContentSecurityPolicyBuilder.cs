// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// Exposes methods to build a content security policy.
    /// </summary>
    public class ContentSecurityPolicyBuilder
    {
        private static readonly IDictionary<SandboxPermission, string> SandboxPermissionValues = new Dictionary<SandboxPermission, string>
        {
            { SandboxPermission.AllowForms, "allow-forms" },
            { SandboxPermission.AllowModals, "allow-modals" },
            { SandboxPermission.AllowOrientationLock, "allow-orientation-lock" },
            { SandboxPermission.AllowPointerLock, "allow-pointer-lock" },
            { SandboxPermission.AllowPopups, "allow-popups" },
            { SandboxPermission.AllowPopupsToEscapeSandbox, "allow-popups-to-escape-sandbox" },
            { SandboxPermission.AllowPresentation, "allow-presentation" },
            { SandboxPermission.AllowSameOrigin, "allow-same-origin" },
            { SandboxPermission.AllowScripts, "allow-scripts" },
            { SandboxPermission.AllowTopNavigation, "allow-top-navigation" }
        };

        private static readonly IDictionary<Subresource, string> SubresourceValues = new Dictionary<Subresource, string>
        {
            { Subresource.Script, "script" },
            { Subresource.Style, "style" }
        };

        private readonly ContentSecurityPolicy _policy = new ContentSecurityPolicy();

        /// <summary>
        /// Adds a default-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddDefaultSrc(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.DefaultSrc, configureDirective);

        /// <summary>
        /// Adds a connect-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddConnectSrc(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.ConnectSrc, configureDirective);

        /// <summary>
        /// Adds a font-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddFontSrc(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.FontSrc, configureDirective);

        /// <summary>
        /// Adds a frame-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddFrameSrc(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.FrameSrc, configureDirective);

        /// <summary>
        /// Adds a image-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddImageSrc(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.ImageSrc, configureDirective);

        /// <summary>
        /// Adds a manifest-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddManifestSrc(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.ManifestSrc, configureDirective);

        /// <summary>
        /// Adds a media-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddMediaSrc(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.MediaSrc, configureDirective);

        /// <summary>
        /// Adds a object-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddObjectSrc(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.ObjectSrc, configureDirective);

        /// <summary>
        /// Adds a script-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddScriptSrc(Action<CspScriptDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.ScriptSrc, configureDirective);

        /// <summary>
        /// Adds a style-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddStyleSrc(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.StyleSrc, configureDirective);

        /// <summary>
        /// Adds a worker-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddWorkerSrc(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.WorkerSrc, configureDirective);

        /// <summary>
        /// Adds a base-uri directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddBaseUri(Action<CspDirectiveBuilder> configureDirective) 
            => AddDirective(CspDirectiveNames.BaseUri, configureDirective);

        /// <summary>
        /// Adds a plugin-types directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="mimeTypes">The valid mime types to allow.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddPluginTypes(params string[] mimeTypes)
            => AddDirective(CspDirectiveNames.PluginTypes, mimeTypes);

        /// <summary>
        /// Adds a sandbox directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="sandboxPermissions">The permissions to be applied to the web page's actions.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddSandbox(params SandboxPermission[] sandboxPermissions)
            => AddDirective(CspDirectiveNames.Sandbox, GetSandboxValues(sandboxPermissions), supportsMetaTag: false, supportsReportHeader: false);

        /// <summary>
        /// Adds a form-action directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddFormAction(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.FormAction, configureDirective);

        /// <summary>
        /// Adds a frame-ancestors directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="configureDirective">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder AddFrameAncestors(Action<CspDirectiveBuilder> configureDirective)
            => AddDirective(CspDirectiveNames.FrameAncestors, configureDirective, supportsMetaTag: false);

        /// <summary>
        /// Adds a block-all-mixed-content directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder BlockAllMixedContent() 
            => AddDirective(CspDirectiveNames.BlockAllMixedContent);

        /// <summary>
        /// Adds a worker-src directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="subresources">A delegate which can use a directive builder to build a directive.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder RequireSubresourceIntegrity(params Subresource[] subresources)
            => AddDirective(CspDirectiveNames.RequireSriFor, GetSubresourceValues(subresources));

        /// <summary>
        /// Adds a upgrade-insecure-requests directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder UpgradeInsecureRequests() 
            => AddDirective(CspDirectiveNames.UpgradeInsecureRequests);

        /// <summary>
        /// Adds a report-uri directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="uri">The uri the browser will send violation reports to.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder ReportUri(string uri)
            => AddDirective(CspDirectiveNames.ReportUri, new[] { uri }, supportsMetaTag: false);

        /// <summary>
        /// Adds a report-to directive to the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="group">The reporting group the browser will send violation reports to.</param>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder ReportTo(string group)
            => AddDirective(CspDirectiveNames.ReportTo, new[] { group }, supportsMetaTag: false);

        /// <summary>
        /// Specifies that the <see cref="ContentSecurityPolicy"/> should only report policy violations instead of enforcing them.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public ContentSecurityPolicyBuilder ReportOnly()
        {
            _policy.ReportOnly = true;
            return this;
        }

        protected virtual ContentSecurityPolicyBuilder AddDirective<TBuilder>(
            string directiveName, 
            Action<TBuilder> configureDirective, 
            bool supportsMetaTag = true,
            bool supportsReportHeader = true) where TBuilder : CspDirectiveBuilder, new()
        {
            if (directiveName == null)
            {
                throw new ArgumentNullException(nameof(directiveName));
            }

            var directiveBuilder = new TBuilder();
            configureDirective?.Invoke(directiveBuilder);

            var directive = directiveBuilder.Build();
            directive.SupportsMetaTag = supportsMetaTag;
            directive.SupportsReportHeader = supportsReportHeader;

            _policy.AddDirective(directiveName, directive);
            
            return this;
        }

        protected virtual ContentSecurityPolicyBuilder AddDirective(
            string directiveName, 
            IEnumerable<string> values = null,
            bool supportsMetaTag = true,
            bool supportsReportHeader = true)
        {
            if (directiveName == null)
            {
                throw new ArgumentNullException(nameof(directiveName));
            }

            var directive = new CspDirective
            {
                SupportsMetaTag = supportsMetaTag,
                SupportsReportHeader = supportsReportHeader
            };

            if (values != null)
            {
                directive.Append(values.ToArray());
            }

            _policy.AddDirective(directiveName, directive);

            return this;
        }

        private IEnumerable<string> GetSandboxValues(SandboxPermission[] sandboxPermissions)
        {
            foreach (var permission in sandboxPermissions)
            {
                yield return SandboxPermissionValues[permission];
            }
        }

        private IEnumerable<string> GetSubresourceValues(Subresource[] subresources)
        {
            foreach (var subresource in subresources)
            {
                yield return SubresourceValues[subresource];
            }
        }

        /// <summary>
        /// Builds a new <see cref="ContentSecurityPolicy"/> using the directives added.
        /// </summary>
        /// <returns>The constructed <see cref="ContentSecurityPolicy"/>.</returns>
        public ContentSecurityPolicy Build()
        {
            return _policy;
        }
    }
}