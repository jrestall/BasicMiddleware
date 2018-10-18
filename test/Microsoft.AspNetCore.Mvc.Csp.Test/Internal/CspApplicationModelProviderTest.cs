// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.AspNetCore.Csp;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Csp.Internal
{
    public class CorsApplicationModelProviderTest
    {
        [Fact]
        public void CreateControllerModel_EnableCorsAttributeAddsCorsAuthorizationFilterFactory()
        {
            // Arrange
            var cspProvider = new CspApplicationModelProvider();
            var context = GetProviderContext(typeof(CspController));

            // Act
            cspProvider.OnProvidersExecuting(context);

            // Assert
            var model = Assert.Single(context.Result.Controllers);
            Assert.Single(model.Filters, f => f is CspAuthorizationFilterFactory);
        }

        [Fact]
        public void CreateControllerModel_DisableCspAttributeAddsDisableCspFilter()
        {
            // Arrange
            var cspProvider = new CspApplicationModelProvider();
            var context = GetProviderContext(typeof(DisableCspController));

            // Act
            cspProvider.OnProvidersExecuting(context);

            // Assert
            var model = Assert.Single(context.Result.Controllers);
            Assert.Single(model.Filters, f => f is DisableCspFilter);
        }

        [Fact]
        public void BuildActionModel_EnableCspAttributeAddsCspAuthorizationFilterFactory()
        {
            // Arrange
            var cspProvider = new CspApplicationModelProvider();
            var context = GetProviderContext(typeof(EnableCspController));

            // Act
            cspProvider.OnProvidersExecuting(context);

            // Assert
            var controller = Assert.Single(context.Result.Controllers);
            var action = Assert.Single(controller.Actions);
            Assert.Single(action.Filters, f => f is CspAuthorizationFilterFactory);
        }

        [Fact]
        public void BuildActionModel_DisableCspAttributeAddsDisableCspFilter()
        {
            // Arrange
            var cspProvider = new CspApplicationModelProvider();
            var context = GetProviderContext(typeof(DisableCspActionController));

            // Act
            cspProvider.OnProvidersExecuting(context);

            // Assert
            var controller = Assert.Single(context.Result.Controllers);
            var action = Assert.Single(controller.Actions);
            Assert.Contains(action.Filters, f => f is DisableCspFilter);
        }

        [Fact]
        public void BuildActionModel_AppendCspAttributeAddsModifyCspFilter()
        {
            // Arrange
            var cspProvider = new CspApplicationModelProvider();
            var context = GetProviderContext(typeof(AppendCspActionController));

            // Act
            cspProvider.OnProvidersExecuting(context);

            // Assert
            var controller = Assert.Single(context.Result.Controllers);
            var action = Assert.Single(controller.Actions);
            var filterMetadata = Assert.Single(action.Filters, f => f is ModifyCspFilter);
            var filter = Assert.IsType<ModifyCspFilter>(filterMetadata);
            Assert.False(filter.OverrideDirectives);
        }

        [Fact]
        public void BuildActionModel_OverrideCspAttributeAddsModifyCspFilter()
        {
            // Arrange
            var cspProvider = new CspApplicationModelProvider();
            var context = GetProviderContext(typeof(OverrideCspActionController));

            // Act
            cspProvider.OnProvidersExecuting(context);

            // Assert
            var controller = Assert.Single(context.Result.Controllers);
            var action = Assert.Single(controller.Actions);
            var filterMetadata = Assert.Single(action.Filters, f => f is ModifyCspFilter);
            var filter = Assert.IsType<ModifyCspFilter>(filterMetadata);
            Assert.True(filter.OverrideDirectives);
        }


        private static ApplicationModelProviderContext GetProviderContext(Type controllerType)
        {
            var context = new ApplicationModelProviderContext(new[] { controllerType.GetTypeInfo() });
            var provider = new DefaultApplicationModelProvider(
                Options.Create(new MvcOptions()),
                new EmptyModelMetadataProvider());
            provider.OnProvidersExecuting(context);

            return context;
        }

        private class EnableCspController
        {
            [EnableCsp("policy")]
            [HttpGet]
            public IActionResult Action()
            {
                return null;
            }
        }

        private class DisableCspActionController
        {
            [DisableCsp]
            [HttpGet]
            public void Action()
            {
            }
        }

        [EnableCsp("policy")]
        public class CspController
        {
            [HttpGet]
            public IActionResult Action()
            {
                return null;
            }
        }

        [DisableCsp]
        public class DisableCspController
        {
            [HttpOptions]
            public IActionResult Action()
            {
                return null;
            }
        }

        public class AppendCspActionController
        {
            [AppendCsp("policy")]
            [HttpPost]
            public IActionResult Action()
            {
                return null;
            }
        }

        public class OverrideCspActionController
        {
            [OverrideCsp("policy")]
            [HttpPost]
            public IActionResult Action()
            {
                return null;
            }
        }
    }
}