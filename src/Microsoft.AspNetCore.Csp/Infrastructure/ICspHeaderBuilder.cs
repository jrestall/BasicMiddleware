using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// A type which can build a response header for a particular <see cref="ContentSecurityPolicy"/>.
    /// </summary>
    public interface ICspHeaderBuilder
    {
        /// <summary>
        /// Returns a <see cref="CspHeader"/> that represents the <paramref name="policy"/> instance.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> associated with the call.</param>
        /// <param name="policy">The <see cref="ContentSecurityPolicy"/> to get the header value for.</param>
        /// <param name="supportMetaTag">True if the header should only include directives that support the HTML meta tag; otherwise false.</param>
        /// <returns>The <see cref="CspHeader"/> that represents the <paramref name="policy"/>.</returns>
        CspHeader GetHeader(HttpContext httpContext, ContentSecurityPolicy policy, bool supportMetaTag = false);
    }
}