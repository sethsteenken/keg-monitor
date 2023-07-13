using KegMonitor.Web.Application;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace KegMonitor.Web.Controllers
{
    public class HealthController : Controller
    {
        private readonly IHealthChecker _healthChecker;

        public HealthController(IHealthChecker healthChecker)
        {
            _healthChecker = healthChecker;
        }

        public async Task<IActionResult> Index()
        {
            var healthy = await _healthChecker.CheckAsync(HttpContext.RequestAborted);
            var responseStatusCode = healthy ? HttpStatusCode.Accepted : HttpStatusCode.InternalServerError;
            return new StatusCodeResult((int)responseStatusCode);
        }
    }
}
