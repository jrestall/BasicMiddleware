// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Microsoft.AspNetCore.Mvc.Csp.Internal
{
    public class CspApplicationModelProvider : IApplicationModelProvider
    {
        public int Order => -1000 + 50;

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            // Intentionally empty.
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            foreach (var controllerModel in context.Result.Controllers)
            { 
                var enableCsp = controllerModel.Attributes.OfType<IEnableCspAttribute>().FirstOrDefault();
                if (enableCsp != null)
                {
                    controllerModel.Filters.Add(new CspActionFilterFactory(enableCsp.PolicyNames));
                }

                var disableCsp = controllerModel.Attributes.OfType<IDisableCspAttribute>().FirstOrDefault();
                if (disableCsp != null)
                {
                    controllerModel.Filters.Add(new DisableCspFilter());
                }

                // TODO: Add all of the attributes instead of just the first.
                var appendCsp = controllerModel.Attributes.OfType<IAppendCspAttribute>().FirstOrDefault();
                if (appendCsp != null)
                {
                    controllerModel.Filters.Add(new ModifyCspFilter(appendCsp.PolicyName, appendCsp.Targets, false));
                }

                var overrideCsp = controllerModel.Attributes.OfType<IOverrideCspAttribute>().FirstOrDefault();
                if (overrideCsp != null)
                {
                    controllerModel.Filters.Add(new ModifyCspFilter(overrideCsp.PolicyName, overrideCsp.Targets, true));
                }

                foreach (var actionModel in controllerModel.Actions)
                {
                    enableCsp = actionModel.Attributes.OfType<IEnableCspAttribute>().FirstOrDefault();
                    if (enableCsp != null)
                    {
                        actionModel.Filters.Add(new CspActionFilterFactory(enableCsp.PolicyNames));
                    }

                    disableCsp = actionModel.Attributes.OfType<IDisableCspAttribute>().FirstOrDefault();
                    if (disableCsp != null)
                    {
                        actionModel.Filters.Add(new DisableCspFilter());
                    }

                    // TODO: Add all of the attributes instead of just the first.
                    appendCsp = actionModel.Attributes.OfType<IAppendCspAttribute>().FirstOrDefault();
                    if (appendCsp != null)
                    {
                        actionModel.Filters.Add(new ModifyCspFilter(appendCsp.PolicyName, appendCsp.Targets, false));
                    }

                    overrideCsp = actionModel.Attributes.OfType<IOverrideCspAttribute>().FirstOrDefault();
                    if (overrideCsp != null)
                    {
                        actionModel.Filters.Add(new ModifyCspFilter(overrideCsp.PolicyName, overrideCsp.Targets, true));
                    }
                }
            }
        }
    }
}
