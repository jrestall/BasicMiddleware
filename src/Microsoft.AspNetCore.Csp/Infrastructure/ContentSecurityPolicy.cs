// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// Defines the policy for a content security policy response header.
    /// </summary>
    public class ContentSecurityPolicy
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        public ContentSecurityPolicy()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ContentSecurityPolicy"/>.
        /// </summary>
        /// <param name="policy">The policy which will be used to initialize the new policy.</param>
        public ContentSecurityPolicy(ContentSecurityPolicy policy)
        {
            Copy(policy);
        }

        public IDictionary<string, CspDirective> Directives { get; } = new Dictionary<string, CspDirective>();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ContentSecurityPolicy"/>
        /// only reports policy violations instead of enforcing them.
        /// </summary>
        /// <value><c>true</c> if report only; otherwise, <c>false</c>.</value>
        public bool ReportOnly { get; set; }

        /// <summary>
        /// The default hash algorithms used when generating hashes for the content security policy.
        /// The default is <see cref="HashAlgorithms.SHA384"/>.
        /// </summary>
        public HashAlgorithms DefaultHashAlgorithms { get; set; } = HashAlgorithms.SHA384;

        /// <summary>
        /// Adds a new policy directive.
        /// </summary>
        /// <param name="name">The name of the directive.</param>
        /// <param name="configureDirective">A delegate which can use a policy builder to build a directive.</param>
        public void AddDirective(string name, Action<CspDirectiveBuilder> configureDirective)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configureDirective == null)
            {
                throw new ArgumentNullException(nameof(configureDirective));
            }

            var directiveBuilder = new CspDirectiveBuilder();
            configureDirective(directiveBuilder);
            Directives[name] = directiveBuilder.Build();
        }

        /// <summary>
        /// Adds a new directive.
        /// </summary>
        /// <param name="name">The name of the directive.</param>
        /// <param name="directive">The <see cref="CspDirective"/> directive to be added.</param>
        public void AddDirective(string name, CspDirective directive)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (directive == null)
            {
                throw new ArgumentNullException(nameof(directive));
            }

            Directives[name] = directive;
        }

        /// <summary>
        /// Gets the directive based on the <paramref name="directiveName"/>.
        /// </summary>
        /// <param name="directiveName">The name of the directive to lookup.</param>
        /// <returns>The <see cref="CspDirective"/> if the directive was added. <c>null</c> otherwise.</returns>
        public CspDirective GetDirective(string directiveName)
        {
            if (directiveName == null)
            {
                throw new ArgumentNullException(nameof(directiveName));
            }

            return Directives.ContainsKey(directiveName) ? Directives[directiveName] : null;
        }

        /// <summary>
        /// Gets the directive based on the <paramref name="directiveName"/> or creates one if it doesn't exist.
        /// </summary>
        /// <param name="directiveName">The name of the directive.</param>
        /// <returns>The existing <see cref="CspDirective"/> if already added; otherwise the added instance.</returns>
        public CspDirective GetOrAddDirective(string directiveName)
        {
            if (directiveName == null)
            {
                throw new ArgumentNullException(nameof(directiveName));
            }

            var directive = GetDirective(directiveName);
            if (directive != null) return directive;

            directive = new CspDirective();
            AddDirective(directiveName, directive);

            return directive;
        }

        /// <summary>
        /// Copies the given <paramref name="policy"/> to the existing properties in the builder.
        /// </summary>
        /// <param name="policy">The policy which needs to be copied.</param>
        private void Copy(ContentSecurityPolicy policy)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            ReportOnly = policy.ReportOnly;
            DefaultHashAlgorithms = policy.DefaultHashAlgorithms;

            foreach (var directive in policy.Directives)
            {
                Directives[directive.Key] = new CspDirective(directive.Value);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var directive in Directives)
            {
                builder.Append(directive);
            }
            return builder.ToString();
        }
    }
}