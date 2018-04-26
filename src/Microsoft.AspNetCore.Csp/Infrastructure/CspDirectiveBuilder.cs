// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// Exposes methods to build a policy directive.
    /// </summary>
    public class CspDirectiveBuilder
    {
        private readonly CspDirective _directive = new CspDirective();

        /// <summary>
        /// Allows the specified <paramref name="host"/> as a source.
        /// </summary>
        /// <param name="host">The host that is allowed.</param>
        /// <returns>The current policy builder.</returns>
        public CspDirectiveBuilder AllowHost(string host) => AllowSource(host);

        /// <summary>
        /// Allows the specified <paramref name="schema"/> as a source.
        /// </summary>
        /// <param name="schema">The schema that is allowed.</param>
        /// <returns>The current policy builder.</returns>
        public CspDirectiveBuilder AllowSchema(string schema) => AllowSource(schema);

        /// <summary>
        /// Allows the same origin as a source.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public CspDirectiveBuilder AllowSelf() => AllowSource("'self'");

        /// <summary>
        /// Allows the use of inline resources, such as inline script and style elements.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public CspDirectiveBuilder AllowUnsafeInline() => AllowSource("'unsafe-inline'");

        /// <summary>
        /// Allows the use of eval() and similar methods that create code from strings.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public CspDirectiveBuilder AllowEval() => AllowSource("'unsafe-eval'");

        /// <summary>
        /// Allows no URLs as a source.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public CspDirectiveBuilder AllowNone() => AllowSource("'none'");

        /// <summary>
        /// Allows scripts or styles with the specified <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash beginning with sha256-, sha384- or sha512-.</param>
        /// <returns>The current policy builder.</returns>
        public CspDirectiveBuilder AllowHash(string hash) 
        {
            if (string.IsNullOrEmpty(hash))
            {
                throw new ArgumentNullException(nameof(hash));
            }

            return AllowSource($"'{hash}'");
        }

        /// <summary>
        /// Allows scripts to be trusted that are loaded from a script protected by a nonce or hash.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public CspDirectiveBuilder UseStrictDynamic() => AllowSource("'strict-dynamic'");

        /// <summary>
        /// Adds a nonce to the <see cref="CspDirective"/>.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public CspDirectiveBuilder AddNonce()
        {
            _directive.AddNonce = true;
            return this;
        }

        /// <summary>
        /// Builds a new <see cref="CspDirective"/> using the entries added.
        /// </summary>
        /// <returns>The constructed <see cref="CspDirective"/>.</returns>
        public CspDirective Build()
        {
            return _directive;
        }

        protected virtual CspDirectiveBuilder AllowSource(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            _directive.Append(source);

            return this;
        }
    }

    public class CspScriptDirectiveBuilder : CspDirectiveBuilder
    {
        /// <summary>
        /// Specifies a sample of the violating code should be included in the violation report.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public CspDirectiveBuilder RequireSampleInReport() => AllowSource("'report-sample'");
    }
}