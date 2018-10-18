namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// An interface which can be used to identify a type which provides metadata needed for appending to content security policies.
    /// </summary>
    public interface IOverrideCspAttribute
    {
        /// <summary>
        /// The name of the override policy.
        /// </summary>
        string PolicyName { get; set; }

        /// <summary>
        /// A comma separated list of the policy names which will be overriden. Defaults to the first active policy if none provided.
        /// </summary>
        string Targets { get; set; }
    }
}