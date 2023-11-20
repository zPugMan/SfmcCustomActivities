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

        
    }
}
