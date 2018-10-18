// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Csp.Infrastructure
{
	/// <summary>
	/// The directive type of the <see cref="CspDirective"/>.
	/// </summary>
	public enum CspDirectiveType
	{
		/// <summary>
		/// Fetch directives control locations from which certain resource types may be loaded.
		/// </summary>
		Fetch,

		/// <summary>
		/// Document directives govern the properties of a document or worker environment to which a policy applies.
		/// </summary>
		Document,

		/// <summary>
		/// Navigation directives govern to which location a user can navigate to or submit a form to.
		/// </summary>
		Navigation,

		/// <summary>
		/// Reporting directives control the reporting process of CSP violations.
		/// </summary>
		Reporting,

		/// <summary>
		/// All other directives that aren't classified into a specific category.
		/// </summary>
		Other
	}
}