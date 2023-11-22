using SfmcCustomActivities.Models.Activities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SfmcCustomActivities.Models.Services
{

    public class SmsRequest
    {
        public SmsRequest(IConfiguration conf) 
        {
            var app = conf.GetSection("Settings:ServiceBus").GetValue<string>("AppName");

            if(string.IsNullOrEmpty(app))
                Source = "SFMC";
            else
                Source = app;
        }

        public SmsRequest(IConfiguration conf, SmsExecute request)
        {
            var app = conf.GetSection("Settings:ServiceBus").GetValue<string>("AppName");

            if (string.IsNullOrEmpty(app))
                Source = "SFMC";
            else
                Source = app;

            if (request.InArguments != null && request.InArguments.Count > 0)
            {
                ToPhone = request.InArguments.First().SmsPhone;
                Message = request.InArguments.First().SmsMessage;
            }

            TrackingID = request.ActivityInstanceId;
        }


        [JsonPropertyName("source")]
        public string? Source { get ; private set; }

        [JsonPropertyName("to")]
        public string? ToPhone { get;set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("trackingID")]
        public string? TrackingID { get; set; }

    }
}
