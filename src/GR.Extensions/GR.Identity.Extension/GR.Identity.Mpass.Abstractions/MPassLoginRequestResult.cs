namespace GR.Identity.Mpass.Abstractions
{
    /// <summary>
    /// The result of sending a login request.
    /// </summary>
    public class MPassLoginRequestResult
    {
        /// <summary>
        /// A base64, digitally signed xml document containing the SAML 2.0 response.
        /// </summary>
        public string Base64SignedXml { get; set; }

        /// <summary>
        /// Indicates wether the response is successful.
        /// </summary>
        public bool Success { get; set; }
    }
}
