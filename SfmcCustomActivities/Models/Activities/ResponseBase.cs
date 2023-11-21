using System.Text.Json.Serialization;

namespace SfmcCustomActivities.Models.Activities
{
    public class ResponseBase
    {
        public ResponseBase()
        {
            Status = "Ok";
            ErrorCode= 0;
        }

        public ResponseBase(string status, int errorCode, string errorMessage)
        {
            Status = status;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
    }
}
