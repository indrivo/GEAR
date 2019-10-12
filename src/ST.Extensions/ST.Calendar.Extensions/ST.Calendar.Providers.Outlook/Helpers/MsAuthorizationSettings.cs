using System.Collections.Generic;

namespace ST.Calendar.Providers.Outlook.Helpers
{
    public class MsAuthorizationSettings
    {
        public MsAuthorizationSettings()
        {
            Scopes.Add("https://graph.microsoft.com/User.Read");
            Scopes.Add("https://graph.microsoft.com/Calendars.Read");
            Scopes.Add("https://graph.microsoft.com/Calendars.ReadWrite");
        }
        /// <summary>
        /// Client id
        /// </summary>
        public virtual string ClientId { get; set; }

        /// <summary>
        /// Client secret id
        /// </summary>
        public virtual string ClientSecretId { get; set; }

        /// <summary>
        /// Tenant id
        /// </summary>
        public virtual string TenantId { get; set; }

        /// <summary>
        /// Instance url
        /// </summary>
        public virtual string InstanceUrl { get; set; } = "https://login.microsoftonline.com/";

        /// <summary>
        /// Api version
        /// </summary>
        public virtual string ApiVersion { get; set; } = "v2.0";

        /// <summary>
        /// Redirect uri
        /// </summary>
        public virtual string RedirectUri { get; set; }

        /// <summary>
        /// Scopes
        /// </summary>
        public IList<string> Scopes { get; } = new List<string>();

        /// <summary>
        /// Get authority
        /// </summary>
        /// <returns></returns>
        public virtual string GetAuthority()
        {
            return $"{InstanceUrl}{TenantId}/{ApiVersion}/";
        }
    }

    public static class OutlookAuthSettings
    {
        /// <summary>
        /// Data
        /// </summary>
        private static MsAuthorizationSettings Data { get; set; }

        /// <summary>
        /// Set settings
        /// </summary>
        /// <param name="settings"></param>
        public static void SetAuthSettings(MsAuthorizationSettings settings)
        {
            Data = settings;
        }


        /// <summary>
        /// Get auth settings
        /// </summary>
        /// <returns></returns>
        public static MsAuthorizationSettings GetAuthSettings()
        {
            return Data;
        }
    }
}