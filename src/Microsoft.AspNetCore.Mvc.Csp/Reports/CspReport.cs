// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Mvc.Csp.Reports
{
    public class CspReport
    {
        [JsonProperty("document-uri")]
        public string DocumentUri { get; set; }

        [JsonProperty("referrer")]
        public string Referrer { get; set; }

        [JsonProperty("violated-directive")]
        public string ViolatedDirective { get; set; }

        [JsonProperty("effective-directive")]
        public string EffectiveDirective { get; set; }

        [JsonProperty("original-policy")]
        public string OriginalPolicy { get; set; }

        [JsonProperty("disposition")]
        public string Disposition { get; set; }

        [JsonProperty("blocked-uri")]
        public string BlockedUri { get; set; }

        [JsonProperty("line-number")]
        public long LineNumber { get; set; }

        [JsonProperty("source-file")]
        public string SourceFile { get; set; }

        [JsonProperty("status-code")]
        public long StatusCode { get; set; }

        [JsonProperty("script-sample")]
        public string ScriptSample { get; set; }
    }
}