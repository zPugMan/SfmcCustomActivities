using Microsoft.AspNetCore.Mvc;
using SfmcCustomActivities.Helpers;
using SfmcCustomActivities.Models.Activities;

namespace SfmcCustomActivities.Controllers.Activities
{
    [DisableSwagger]
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

        [HttpGet()]
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
