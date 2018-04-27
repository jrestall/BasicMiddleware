// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// Defines the policy for Cross-Origin requests based on the CORS specifications.
    /// </summary>
    public class CspDirective
    {
        private StringBuilder _directiveValue;

        /// <summary>
        /// Creates a new instance of the <see cref="CspDirective"/>.
        /// </summary>
        public CspDirective()
        {
            _directiveValue = new StringBuilder();
            
            SupportsMetaTag = true;
            SupportsReportHeader = true;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CspDirective"/>.
        /// </summary>
        /// <param name="directive">The policy which will be used to initialize the new directive.</param>
        public CspDirective(CspDirective directive)
        {
            Copy(directive);
        }

        /// <summary>
        /// True if this <see cref="CspDirective"/> is supported within HTML meta tags, otherwise false.
        /// </summary>
        public bool SupportsMetaTag { get; set; }

        /// <summary>
        /// True if this <see cref="CspDirective"/> is supported within Content-Security-Policy-Report-Only headers, otherwise false.
        /// </summary>
        public bool SupportsReportHeader { get; set; }

        /// <summary>
        /// True if this <see cref="CspDirective"/> should have a nonce, otherwise false.
        /// </summary>
        public bool AddNonce { get; set; }

        /// <summary>
        /// The concatenated set of non-empty strings.
        /// </summary>
        public string Value => _directiveValue.ToString();

        /// <summary>
        /// Adds the given source values to the directive.
        /// </summary>
        /// <param name="values">The sources to be added.</param>
        /// <remarks>Directive values should only contain whitespace and visible characters, excluding ";" and ",".</remarks>
        public void Append(params string[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length == 0)
            {
                return;
            }

            var separator = " ";
            if (_directiveValue.Length == 0) separator = string.Empty;

            _directiveValue.Append(separator + string.Join(" ", values));
        }

        /// <summary>
        /// Adds the given source values to the directive and encloses each in single quotes e.g. 'sha512-12345'.
        /// </summary>
        /// <param name="values">The values to be added.</param>
        public void AppendQuoted(params string[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            foreach (var value in values)
            {
                Append($"'{value}'");
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
            builder.Append("Value: ");
            builder.Append(Value);
            builder.Append(", SupportsMetaTag: ");
            builder.Append(SupportsMetaTag);
            builder.Append(", SupportsReportHeader: ");
            builder.Append(SupportsReportHeader);
            builder.Append(", AddNonce: ");
            builder.Append(AddNonce);
            return builder.ToString();
        }

        /// <summary>
        /// Copies the given <paramref name="directive"/> to the existing properties in the current directive.
        /// </summary>
        /// <param name="directive">The directive which will be copied.</param>
        private void Copy(CspDirective directive)
        {
            _directiveValue = new StringBuilder(directive.Value);

            SupportsMetaTag = directive.SupportsMetaTag;
            SupportsReportHeader = directive.SupportsReportHeader;
            AddNonce = directive.AddNonce;
        }
    }
}