namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// The header representing a <see cref="ContentSecurityPolicy"/> instance.
    /// </summary>
    public class CspHeader
    {
        /// <summary>
        /// The name of the header, either Content-Security-Policy or Content-Security-Policy-Report-Only.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The space delimited list of directive values that represent the content security policy.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="CspHeader"/>.
        /// </summary>
        /// <param name="name">The name of the content security policy header.</param>
        /// <param name="value">The space delimited list of directive values that represent the content security policy.</param>
        public CspHeader(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}