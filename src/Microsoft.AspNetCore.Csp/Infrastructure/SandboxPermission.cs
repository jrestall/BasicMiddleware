// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    /// <summary>
    /// The restrictions to be applied to a web page's actions.
    /// </summary>
    public enum SandboxPermission
    {
        /// <summary>
        /// Allows the embedded browsing context to submit forms. If this keyword is not used, this operation is not allowed.
        /// </summary>
        AllowForms,
        
        /// <summary>
        /// Allows the embedded browsing context to open modal windows.
        /// </summary>
        AllowModals,
        
        /// <summary>
        /// Allows the embedded browsing context to disable the ability to lock the screen orientation.
        /// </summary>
        AllowOrientationLock,
        
        /// <summary>
        /// Allows the embedded browsing context to use the Pointer Lock API.
        /// </summary>
        AllowPointerLock,
        
        /// <summary>
        /// Allows popups (like from window.open, target="_blank", showModalDialog). If this permission is not used, that functionality will silently fail.
        /// </summary>
        AllowPopups,
        
        /// <summary>
        /// Allows a sandboxed document to open new windows without forcing the sandboxing flags upon them.
        /// </summary>
        AllowPopupsToEscapeSandbox,
        
        /// <summary>
        /// Allows embedders to have control over whether an iframe can start a presentation session.
        /// </summary>
        AllowPresentation,
        
        /// <summary>
        /// Allows the content to be treated as being from its normal origin. If this keyword is not used, the embedded content is treated as being from a unique origin.
        /// </summary>
        AllowSameOrigin,
        
        /// <summary>
        /// Allows the embedded browsing context to run scripts (but not create pop-up windows). If this keyword is not used, this operation is not allowed.
        /// </summary>
        AllowScripts,
        
        /// <summary>
        /// Allows the embedded browsing context to navigate (load) content to the top-level browsing context. If this keyword is not used, this operation is not allowed.
        /// </summary>
        AllowTopNavigation
    }
}
