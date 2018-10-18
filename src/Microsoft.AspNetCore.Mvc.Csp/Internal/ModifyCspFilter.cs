namespace Microsoft.AspNetCore.Mvc.Csp.Internal
{
    /// <summary>
    /// An implementation of <see cref="IDisableCspFilter"/>
    /// </summary>
    public class ModifyCspFilter : IModifyCspFilter
    {
        public string PolicyName { get; }
        public string Targets { get; }
        public bool OverrideDirectives { get; }

        public ModifyCspFilter(string policyName, string targets, bool overrideDirectives)
        {
            PolicyName = policyName;
            Targets = targets;
            OverrideDirectives = overrideDirectives;
        }
    }
}