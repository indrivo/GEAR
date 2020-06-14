namespace GR.Identity.PhoneVerification.Abstractions.Helpers
{
    public class TwilioSettings
    {
        public virtual string AuthyApiKey { get; set; }
        public virtual string AccountSid { get; set; }
        public virtual string AuthToken { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool IsTestEnv { get; set; }
    }
}