using Microsoft.AspNetCore.Mvc;
using SfmcCustomActivities.Models.Activities;
using System.Text.Json;

namespace SfmcCustomActivities.Controllers.Activities
{
    [Route("Activities/api/[controller]/[action]")]
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
            _log.LogInformation($"Publishing: {JsonSerializer.Serialize<SmsPublish>(payload)}");
            return new OkResult();
        }

        [HttpPost]
        public IActionResult Validate(ActivityBase payload)
        {
            _log.LogInformation($"Validating: {JsonSerializer.Serialize<ActivityBase>(payload)}");
            return new OkResult();
        }

        [HttpPost]
        public IActionResult Stop(ActivityBase payload)
        {
            _log.LogInformation($"Stop requested: {JsonSerializer.Serialize<ActivityBase>(payload)}");
            return new OkResult();
        }

        [HttpPost]
        public ActionResult<ResponseBase> Execute(SmsExecute payload)
        {
            _log.LogInformation($"Execute requested: {JsonSerializer.Serialize<SmsExecute>(payload)}");
            return new ResponseBase();
        }
    }
}
