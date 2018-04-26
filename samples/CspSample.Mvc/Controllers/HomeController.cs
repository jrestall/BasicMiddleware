using Microsoft.AspNetCore.Csp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Csp.Reports;

namespace CspSample.Mvc.Controllers
{
    public class HomeController : Controller
    {
        [EnableCsp]
        public IActionResult Enable()
        {
            ViewData["Message"] = "The default policy is enabled by the [EnableCsp] attribute.";

            return View("Index");
        }

        [EnableCsp("Policy1")]
        public IActionResult EnableNamed()
        {
            ViewData["Message"] = "Policy1 is enabled.";

            return View("Index");
        }

        [EnableCsp("Policy1", "Policy2")]
        public IActionResult EnableNamedMultiple()
        {
            ViewData["Message"] = "Policy1 and Policy2 are enabled.";

            return View("Index");
        }

        [DisableCsp]
        public IActionResult Disable()
        {
            ViewData["Message"] = "CSP is disabled by the [DisableCsp] attribute. No CSP headers will be sent.";

            return View("Index");
        }

        public IActionResult CspReports(CspReportRequest request)
        {
            return Ok();
        }
    }
}
