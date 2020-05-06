namespace GR.Identity.PhoneVerification.Abstractions.Helpers
{
    public class TwilioSettings
    {
        public virtual string AuthyApiKey { get; set; }
        public virtual bool IsTestEnv { get; set; }
    }
}