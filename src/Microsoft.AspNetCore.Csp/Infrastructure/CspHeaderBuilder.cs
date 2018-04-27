using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    public class CspHeaderBuilder : ICspHeaderBuilder
    {
        private readonly INonceProvider _nonceProvider;

        /// <summary>
        /// Creates a new instance of <see cref="CspHeaderBuilder"/>.
        /// </summary>
        /// <param name="nonceProvider">The nonce provider that generates nonces for the header.</param>
        public CspHeaderBuilder(INonceProvider nonceProvider)
        {
            _nonceProvider = nonceProvider;
        }

        /// <inheritdoc />
        public CspHeader GetHeader(HttpContext httpContext, ContentSecurityPolicy policy, bool supportMetaTag = false)
        {
            var headerName = GetHeaderName(policy.ReportOnly);
            var headerValue = GetHeaderValue(httpContext, policy, policy.ReportOnly);

            return new CspHeader(headerName, headerValue);
        }

        private string GetHeaderName(bool reportOnly)
        {
            return reportOnly ? HeaderNames.ContentSecurityPolicyReportOnly : HeaderNames.ContentSecurityPolicy;
        }

        private string GetHeaderValue(HttpContext httpContext, ContentSecurityPolicy policy, bool supportMetaTag)
        {
            var builder = new StringBuilder();

            bool isFirstDirective = true;
            foreach (var directiveName in policy.Directives.Keys)
            {
                var directive = policy.Directives[directiveName];

                // Some directives don't support <meta http-equiv="Content-Security-Policy">.
                if (supportMetaTag && !directive.SupportsMetaTag) continue;

                // Some directives don't support the Content-Security-Policy-Report-Only header.
                if (policy.ReportOnly && !directive.SupportsReportHeader) continue;

                // Add the semi-colon separator between directives.
                if (!isFirstDirective) builder.Append("; ");
                isFirstDirective = false;

                // Append the directive name and its space delimited value(s).
                // E.g. default-src https: 'unsafe-eval' 'unsafe-inline'
                builder.Append($"{directiveName}");

                var directiveValue = directive.Value;
                if (!string.IsNullOrEmpty(directiveValue))
                {
                    builder.Append($" {directiveValue}");
                }
                    
                // Add a nonce to the end of the directive
                if (directive.AddNonce)
                {
                    var nonce = _nonceProvider.GetNonce(httpContext);
                    builder.Append($" 'nonce-{nonce}'");
                }
            }

            return builder.ToString();
        }
    }
}