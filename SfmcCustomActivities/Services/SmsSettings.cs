namespace SfmcCustomActivities.Services
{
    public class SmsSettings
    {
        private SmsSettings() { }

        public static SmsSettings Instance { get; protected set; } = new SmsSettings();

        public int ConcurrentRequests { get; set; }
        public bool JWTEnabled { get; set; }

        public string? JWTSecret { get; set;}

    }
}
