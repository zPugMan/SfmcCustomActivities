namespace SfmcCustomActivities.Models.Activities
{
    public class SmsExecute : ActivityBase
    {
        public string? JourneyId { get; set; }
        public string? ActivityId { get; set; }
        public string? DefinitionInstanceId { get; set; }
        public string? ActivityInstanceId { get; set; }
        public string? KeyValue { get; set; }    
        public int? Mode { get; set; }

        public List<ExecuteInputs>? InArguments { get; set; }
        public List<ExecuteOutputs>? OutArguments { get; set; }
    }

    public class ExecuteInputs
    {
        public string? SmsKeyword { get; set; }
        public string? SmsPhone { get; set; }
        public string? SmsMessage { get; set; }
    }

    public class ExecuteOutputs
    {

    }
}
