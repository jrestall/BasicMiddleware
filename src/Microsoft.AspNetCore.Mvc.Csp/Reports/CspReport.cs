// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Mvc.Csp.Reports
{
    public class CspReport
    {
        [JsonProperty(PropertyName = "document-uri")]
        public string DocumentUri { get; set; }

        [JsonProperty(PropertyName = "referrer")]
        public string Referrer { get; set; }

        [JsonProperty(PropertyName = "blocked-uri")]
        public string BlockedUri { get; set; }

        [JsonProperty(PropertyName = "effective-directive")]
        public string EffectiveDirective { get; set; }

        [JsonProperty(PropertyName = "violated-directive")]
        public string ViolatedDirective { get; set; }

        [JsonProperty(PropertyName = "original-policy")]
        public string OriginalPolicy { get; set; }

        [JsonProperty(PropertyName = "disposition")]
        public string Disposition { get; set; }

        [JsonProperty(PropertyName = "status-code")]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "script-sample")]
        public int ScriptSample { get; set; }
    }
}