using KegMonitor.Core;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace KegMonitor.Web.Controllers
{
    public class LogController : Controller
    {
        private readonly ILoggerFactory _loggerFactory;

        public LogController(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        [Route("")]
        [HttpPost]
        public IActionResult Index([FromBody] LogMessage model)
        {
            if (!Request.HasJsonContentType())
                return new StatusCodeResult((int)HttpStatusCode.UnsupportedMediaType);
           
            _loggerFactory.CreateLogger(model.Logger)
                          .Log(model);

            return new StatusCodeResult((int)HttpStatusCode.Accepted);
        }
    }
}
