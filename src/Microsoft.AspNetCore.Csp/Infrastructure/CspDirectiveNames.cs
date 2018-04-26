// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
    public static class CspDirectiveNames
    {
        // Fetch Directives
        public const string ConnectSrc = "connect-src";
        public const string DefaultSrc = "default-src";
        public const string FontSrc = "font-src";
        public const string FrameSrc = "frame-src";
        public const string ImageSrc = "image-src";
        public const string ManifestSrc = "manifest-src";
        public const string MediaSrc = "media-src";
        public const string ObjectSrc = "object-src";
        public const string ScriptSrc = "script-src";
        public const string StyleSrc = "style-src";
        public const string WorkerSrc = "worker-src";

        // Document Directives
        public const string BaseUri = "base-uri";
        public const string PluginTypes = "plugin-types";
        public const string Sandbox = "sandbox";

        // Navigation Directives
        public const string FormAction = "form-action";
        public const string FrameAncestors = "frame-ancestors";

        // Reporting Directives
        public const string ReportUri = "report-uri";
        public const string ReportTo = "report-to";

        // Other Directives
        public const string BlockAllMixedContent = "block-all-mixed-content";
        public const string RequireSriFor = "require-sri-for";
        public const string UpgradeInsecureRequests = "upgrade-insecure-requests";
    }
}