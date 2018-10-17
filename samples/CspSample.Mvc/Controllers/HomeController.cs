using Microsoft.AspNetCore.Csp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Csp.Reports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CspSample.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

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
        //[AppendCsp("BetaUsers", Targets = "Policy1, Policy2")]
        //[OverrideCsp("BetaUsers2", Targets = "Policy1")]
        //[AppendCspDirective("script-src", "'self'", Targets = "Policy1")]
        //[OverrideCspDirective("script-src", "'self'")]
        public IActionResult EnableNamedMultiple()
        {
            ViewData["Message"] = "Policy1 and Policy2 are enabled.";

            return View("Index");
        }

        [EnableCsp("SubresourceIntegrityPolicy")]
        public IActionResult SubresourceIntegrity()
        {
            ViewData["Message"] = "SubresourceIntegrityPolicy is enabled.";

            return View();
        }

        [DisableCsp]
        public IActionResult Disable()
        {
            ViewData["Message"] = "CSP is disabled by the [DisableCsp] attribute. No CSP headers will be sent.";

            return View("Index");
        }

        [HttpPost]
        public IActionResult CspReports([FromBody]CspReportRequest request)
        {
            var report = JsonConvert.SerializeObject(request);
            _logger.LogWarning($"CSP Violation Report: {report}");

            return NoContent();
        }
    }
}
