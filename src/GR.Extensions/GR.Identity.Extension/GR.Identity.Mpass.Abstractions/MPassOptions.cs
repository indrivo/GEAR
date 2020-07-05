namespace GR.Identity.Mpass.Abstractions
{
    /// <summary>
    /// Options of MPass integration, all settings are defaulted
    /// to MPass servers so please keep this in mind when switching to 
    /// Production. When doing so provide a json file configuration 
    /// overriding these ones for production environment.
    /// </summary>
    public class MPassOptions
    {
        // The backing fields will serve as default values
        // if somehow the json configuration files are not found
        // so that the app would not crash.

        #region Backing Fields default values
        /// <summary>
        /// The saml issuer, default to test server
        /// </summary>
        private string _samlIssuer = "http://sample.testmpass.gov.md";

        /// <summary>
        /// The saml destination, default to test server
        /// </summary>
        private string _samlDestination = "http://testmpass.gov.md/login/saml";

        /// <summary>
        /// The saml destination, default to test server
        /// </summary>
        private string _samlLogoutDestination = "http://testmpass.gov.md/logout/saml";

        /// <summary>
        /// The ceritificate password
        /// </summary>
        private string _servicesCertificatePassword = "qN6n31IT86684JO";

        /// <summary>
        /// The assertion url, defaults to local development server
        /// </summary>
        private string _samlAssertionConsumerUrl = "http://localhost:50341/Account/Acs";
        #endregion

        /// <summary>
        /// The Issuer of the certificate
        /// </summary>
        public string SAMLIssuer
        {
            get { return _samlIssuer; }
            set { _samlIssuer = value; }
        }

        /// <summary>
        /// The Assertion consumer url, where the saml
        /// response is parsed, validated and processed
        /// </summary>
        public string SAMLAssertionConsumerUrl
        {
            get { return _samlAssertionConsumerUrl; }
            set { _samlAssertionConsumerUrl = value; }
        }

        /// <summary>
        /// Password of the certificate
        /// </summary>
        public string ServiceCertificatePassword
        {
            get { return _servicesCertificatePassword; }
            set { _servicesCertificatePassword = value; }
        }

        /// <summary>
        /// The actual login request endpoint of MPass, where
        /// a login request will be sent through POST.
        /// </summary>
        public string SAMLDestination
        {
            get { return _samlDestination; }
            set { _samlDestination = value; }
        }

        /// <summary>
        /// The actual Logout request endpoint of MPass, where
        /// a login request will be sent through POST.
        /// </summary>
        public string SAMLLogoutDestination
        {
            get { return _samlLogoutDestination; }
            set { _samlLogoutDestination = value; }
        }
    }
}
