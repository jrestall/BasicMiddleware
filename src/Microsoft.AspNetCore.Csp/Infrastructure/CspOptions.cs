// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// Provides programmatic configuration for Csp.
    /// </summary>
    public class CspOptions
    {
        // Default Content-Security-Policy:
        //     default-src 'self' https:; font-src 'self' https: data:; img-src 'self' https: data:;
        //     object-src 'none'; script-src https:; style-src 'self' https: 'unsafe-inline'
        private static readonly ContentSecurityPolicy DefaultContentSecurityPolicy
            = new ContentSecurityPolicyBuilder()
                .AddDefaultSrc(src =>
                {
                    src.AllowSelf();
                    src.AllowSchema(CspDirectiveSchemas.Https);
                })
                .AddFontSrc(src =>
                {
                    src.AllowSelf();
                    src.AllowSchema(CspDirectiveSchemas.Https);
                    src.AllowSchema(CspDirectiveSchemas.Data);
                })
                .AddImageSrc(src =>
                {
                    src.AllowSelf();
                    src.AllowSchema(CspDirectiveSchemas.Https);
                    src.AllowSchema(CspDirectiveSchemas.Data);
                })
                .AddObjectSrc(src =>
                {
                    src.AllowNone();
                })
                .AddScriptSrc(src =>
                {
                    src.AllowSelf();
                    src.AllowSchema(CspDirectiveSchemas.Https);
                })
                .AddStyleSrc(src =>
                {
                    src.AllowSelf();
                    src.AllowSchema(CspDirectiveSchemas.Https);
                    src.AllowUnsafeInline();
                })
                .Build();
        
        private const string OriginalDefaultPolicyName = "__DefaultContentSecurityPolicy";

        private string _defaultPolicyName = OriginalDefaultPolicyName;

        private IDictionary<string, ContentSecurityPolicy> PolicyMap { get; } = new Dictionary<string, ContentSecurityPolicy>
        {
            {OriginalDefaultPolicyName, DefaultContentSecurityPolicy}
        };

        public string DefaultPolicyName
        {
            get
            {
                return _defaultPolicyName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _defaultPolicyName = value;
            }
        }

        /// <summary>
        /// Adds a new policy and sets it as the default.
        /// </summary>
        /// <param name="policy">The <see cref="ContentSecurityPolicy"/> policy to be added.</param>
        public void AddDefaultPolicy(ContentSecurityPolicy policy)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            AddPolicy(DefaultPolicyName, policy);
        }

        /// <summary>
        /// Adds a new policy and sets it as the default.
        /// </summary>
        /// <param name="configurePolicy">A delegate which can use a policy builder to build a policy.</param>
        public void AddDefaultPolicy(Action<ContentSecurityPolicyBuilder> configurePolicy)
        {
            if (configurePolicy == null)
            {
                throw new ArgumentNullException(nameof(configurePolicy));
            }

            AddPolicy(DefaultPolicyName, configurePolicy);
        }

        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="policy">The <see cref="ContentSecurityPolicy"/> policy to be added.</param>
        public void AddPolicy(string name, ContentSecurityPolicy policy)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            PolicyMap[name] = policy;
        }

        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="configurePolicy">A delegate which can use a policy builder to build a policy.</param>
        public void AddPolicy(string name, Action<ContentSecurityPolicyBuilder> configurePolicy)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configurePolicy == null)
            {
                throw new ArgumentNullException(nameof(configurePolicy));
            }

            var policyBuilder = new ContentSecurityPolicyBuilder();
            configurePolicy(policyBuilder);
            PolicyMap[name] = policyBuilder.Build();
        }

        /// <summary>
        /// Gets the policy based on the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the policy to lookup.</param>
        /// <returns>The <see cref="ContentSecurityPolicy"/> if the policy was added. <c>null</c> otherwise.</returns>
        public ContentSecurityPolicy GetPolicy(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return PolicyMap.ContainsKey(name) ? PolicyMap[name] : null;
        }
    }
}