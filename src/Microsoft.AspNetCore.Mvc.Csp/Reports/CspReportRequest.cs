﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Mvc.Csp.Reports
{
    public class CspReportRequest
    {
        [JsonProperty("csp-report")]
        public CspReport CspReport { get; set; }
    }
}