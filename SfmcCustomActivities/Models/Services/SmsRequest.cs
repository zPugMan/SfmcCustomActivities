using SfmcCustomActivities.Models.Activities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SfmcCustomActivities.Models.Services
{

    public class SmsRequest
    {
        private readonly IConfiguration _conf;
        private readonly ILogger _log;
        private const string DEFAULT_APP = "SFMC";

        public SmsRequest(IConfiguration conf, ILogger log) 
        {
            _conf = conf;
            _log = log;
            Init();
        }

        public SmsRequest(IConfiguration conf, ILogger log, SmsExecute request)
        {
            _conf = conf;
            _log = log;
            Init();

            if (request.InArguments != null && request.InArguments.Count > 0)
            {
                ToPhone = request.InArguments.First().SmsPhone;
                Message = request.InArguments.First().SmsMessage;
            }

            TrackingID = request.ActivityInstanceId;
        }

        private void Init()
        {
            string app = string.Empty;
            try
            {
                app = _conf.GetSection("Settings:ServiceBus").GetValue<string>("AppName")!;
            }
            catch (NullReferenceException e)
            {
                _log.LogWarning($"No value for `Settings:ServiceBus:AppName`. Using default: {DEFAULT_APP}");
            }

            if (string.IsNullOrEmpty(app))
                Source = DEFAULT_APP;
            else
                Source = app;
        }

        public bool IsValid()
        {
            if(string.IsNullOrEmpty(ToPhone))
                return false;

            if(string.IsNullOrEmpty(Message)) 
                return false;

            if(string.IsNullOrEmpty(Source)) 
                return false;

            if(ToPhone.Length < 10)
                return false;

            return true;
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
