using System.Text.Json.Serialization;

namespace SfmcCustomActivities.Models.Activities
{

    public class ActivityBase
    {
        [JsonPropertyName("activityObjectID")]
        [JsonRequired]
        public string? ActivityObjectId { get; set; }

        public string? InteractionId { get; set; }

        public string? originalDefinitionId { get;set; }

        public string? interactionKey { get; set; }
        public string? interactionVersion { get; set; }

    }
}
