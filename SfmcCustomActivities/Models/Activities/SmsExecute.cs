using System.Text.Json.Serialization;
namespace SfmcCustomActivities.Models.Activities
{
    public class SmsExecute : ActivityBase
    {
        [JsonPropertyName("journeyId")]
        public string? JourneyId { get; set; }

        [JsonPropertyName("activityId")]
        public string? ActivityId { get; set; }

        [JsonPropertyName("definitionInstanceId")]
        public string? DefinitionInstanceId { get; set; }

        [JsonPropertyName("activityInstanceId")]
        public string? ActivityInstanceId { get; set; }

        [JsonPropertyName("keyValue")]
        public string? KeyValue { get; set; }

        [JsonPropertyName("mode")]
        public int? Mode { get; set; }

        [JsonPropertyName("inArguments")]
        public List<ExecuteInputs>? InArguments { get; set; }

        [JsonPropertyName("outArguments")]
        public List<ExecuteOutputs>? OutArguments { get; set; }
    }

    public class ExecuteInputs
    {
        [JsonPropertyName("smsKeyword")]
        public string? SmsKeyword { get; set; }

        [JsonPropertyName("smsPhone")]
        public string? SmsPhone { get; set; }

        [JsonPropertyName("smsMessage")]
        public string? SmsMessage { get; set; }
    }

    public class ExecuteOutputs
    {

    }
}
