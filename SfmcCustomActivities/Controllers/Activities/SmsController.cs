using Microsoft.AspNetCore.Mvc;
using SfmcCustomActivities.Models.Activities;

namespace SfmcCustomActivities.Controllers.Activities
{
    [Route("Activities/[controller]")]
    public class SmsController : Controller
    {
        IWebHostEnvironment _hostEnv;
        private readonly ILogger _log;

        public SmsController(IWebHostEnvironment hostEnv, ILogger<SmsController> logger)
        {
            _hostEnv= hostEnv;
            _log = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("config.json")]
        [Produces("application/json")]
        public IActionResult Configuration()
        {
            var result = SmsConfig.GetConfigJson(this.HttpContext, _hostEnv);

            if (result==null)
            {
                _log.LogWarning("Retrieval of 'config.json' failed");
                return NotFound(result);
            }

            return new OkObjectResult(result);
        }

        [HttpPost]
        public IActionResult Publish(SmsPublish payload)
        {
            _log.LogDebug($"Publishing: {payload}");
            return new OkResult();
        }

        [HttpPost]
        public IActionResult Validate(ActivityBase payload)
        {
            _log.LogDebug($"Validating: {payload}");
            return new OkResult();
        }

        [HttpPost]
        public IActionResult Stop(ActivityBase payload)
        {
            _log.LogDebug($"Stop requested: {payload}");
            return new OkResult();
        }

        [HttpPost]
        public IActionResult Execute(SmsExecute payload)
        {
            return new OkResult();
        }
    }
}
