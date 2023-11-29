using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using SfmcCustomActivities.Models.Activities;
using SfmcCustomActivities.Models.Services;
using System.Text.Json;
using Azure.Core.Extensions;
using Microsoft.Extensions.Azure;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SfmcCustomActivities.Controllers.Activities
{
    [Route("Activities/api/[controller]/[action]")]
    [ApiController]
    public class SmsApiController : ControllerBase
    {
        private readonly ILogger _log;
        private readonly ServiceBusSender _azureSend;
        private readonly IConfiguration _conf;

        public SmsApiController(ILogger<SmsApiController> log, IAzureClientFactory<ServiceBusSender> azureSend, IConfiguration conf)
        {
            _log = log;
            _azureSend = azureSend.CreateClient("SendQueue");
            _conf = conf;
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
        public async Task<ActionResult<ResponseBase>> Execute(SmsExecute payload)
        {
            _log.LogInformation($"Execute requested: {JsonSerializer.Serialize<SmsExecute>(payload)}");

            var result = new ResponseBase();
            try
            {
                SmsRequest sms = new SmsRequest(_conf, _log, payload);
                if (!sms.IsValid())
                {
                    _log.LogWarning($"{nameof(SmsRequest)} is invalid.. returning failure");
                    return new BadRequestObjectResult(new ResponseBase(status: "Error", errorCode: 400, errorMessage: "Missing required SMS information"));
                }


                var msg = new ServiceBusMessage(JsonSerializer.Serialize<SmsRequest>(sms));
                msg.ContentType = "application/json";
                msg.CorrelationId = payload.ActivityInstanceId;

                await _azureSend.SendMessageAsync(msg);
                _log.LogInformation($"Response: {JsonSerializer.Serialize<ResponseBase>(result)}");
                return result;

            } catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                result.ErrorCode = -500;
                result.Status = "Fail";
                _log.LogWarning($"Response: {JsonSerializer.Serialize<ResponseBase>(result)}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            
            
             
        }
    }
}
