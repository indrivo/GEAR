namespace GR.Core.Helpers.ErrorCodes
{
    public static class ResultModelCodes
    {
        /// <summary>
        /// Not found
        /// </summary>
        public static string NotFound = "G404";

        /// <summary>
        /// Forbidden
        /// </summary>
        public static string Forbidden = "G403";

        /// <summary>
        /// Not authorized
        /// </summary>
        public static string NotAuthorized = "G401";

        /// <summary>
        /// Internal server error
        /// </summary>
        public static string InternalError = "G500";

        /// <summary>
        /// Email not confirmed
        /// </summary>
        public static string EmailNotConfirmed = "G-email-not-confirmed";
    }
}
