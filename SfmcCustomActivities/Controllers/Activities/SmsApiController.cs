using Microsoft.AspNetCore.Mvc;
using SfmcCustomActivities.Models.Activities;

namespace SfmcCustomActivities.Controllers.Activities
{
    [Route("Activities/api/[controller]")]
    [ApiController]
    public class SmsApiController : ControllerBase
    {
        private readonly ILogger _log;

        public SmsApiController(ILogger<SmsApiController> log)
        {
            _log = log;
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
