using System;
using System.Text;
using GR.Identity.Mpass.Abstractions;
using GR.Identity.Mpass.Abstractions.Helpers;
using GR.Identity.Mpass.Abstractions.Security;
using GR.Identity.Mpass.ParsingAndValidation;
using Microsoft.Extensions.Options;

namespace GR.Identity.Mpass
{
    /// <summary>
    /// Service that will generate MPass (http://mpass.gov.md) login requests
    /// and process responses.
    /// </summary>
    public class MPassService : IMPassService
    {
        #region DependencyInjection private fields
        private readonly MPassSigningCredentials _mPassSigningCredentials;
        private readonly MPassOptions _mPassOptions;
        #endregion

        /// <summary>
        /// The main constructor that get's required services from DI Container
        /// </summary>
        /// <param name="store">Store of MPass Credentials (certificates)</param>
        /// <param name="mpassOptions">MPass Service Configurations</param>
        public MPassService(IMPassSigningCredentialsStore store, IOptionsSnapshot<MPassOptions> mpassOptions)
        {
            _mPassSigningCredentials = store.GetMPassCredentials();
            _mPassOptions = mpassOptions.Value;
        }

        /// <summary>
        /// Parse and retrieve <see cref="MPassUserInfo"/> from 
        /// the login response
        /// </summary>
        /// <param name="samlResponse">Response recieved after requesting a login</param>
        /// <returns><see cref="MPassUserInfo"/> object containing all the data or null if information retrieval is unsuccessful</returns>
        /// <exception cref="FormatException">May be thrown by <see cref="Convert.ToBase64String(byte[])"/></exception>
        /// <exception cref="DecoderFallbackException">May be thrown by <see cref="Encoding.UTF8"/></exception>
        public MPassUserInfo GetMPassUserInfoFromLoginResponse(string samlResponse)
        {
            if (string.IsNullOrEmpty(samlResponse))
                return null;

            var responseBytes = Convert.FromBase64String(samlResponse);
            var xml = Encoding.UTF8.GetString(responseBytes);

            return MPassResponseParsing.GetMPassUserFromXml(xml);
        }

        /// <summary>
        /// Generate a MPass login request that can be used to initialize the process 
        /// of MPass Login. It should pe sent through POST method to http://mpass.gov.md/login/saml
        /// with a field in formdata named SAMLRequest that has a base 64 signed login request xml 
        /// document
        /// </summary>
        /// <returns>A SAML login request</returns>
        public string GetMPassRequest(string requestId)
        {
            
            var samlRequest = SamlMessage.BuildAuthnRequest(requestId, _mPassOptions.SAMLDestination, _mPassOptions.SAMLAssertionConsumerUrl, _mPassOptions.SAMLIssuer);
            samlRequest = SamlMessage.Sign(samlRequest, _mPassSigningCredentials.ServiceProviderCertificate);
            var dataBytes = Encoding.UTF8.GetBytes(samlRequest);
            return Convert.ToBase64String(dataBytes);
        }

        /// <summary>
        /// Generate a MPass logout request that can be used to initialize the process 
        /// of MPass logout. It should pe sent through POST method to http://mpass.gov.md/logout/saml
        /// with a field in formdata named SAMLRequest that has a base 64 signed login request xml 
        /// document
        /// </summary>
        /// <param name="nameId"></param>
        /// <param name="sessionIndex"></param>
        ///  <param name="requestId"></param>
        /// <returns>A SAML logout request</returns>
        public string GetMPassLogoutRequest(string requestId,string nameId, string sessionIndex)
        {
            var samlRequest = SamlMessage.BuildLogoutRequest(requestId, _mPassOptions.SAMLLogoutDestination,  _mPassOptions.SAMLIssuer , nameId , sessionIndex);
            samlRequest = SamlMessage.Sign(samlRequest, _mPassSigningCredentials.ServiceProviderCertificate);
            var dataBytes = Encoding.UTF8.GetBytes(samlRequest);
            return Convert.ToBase64String(dataBytes);
        }

        /// <summary>
        /// Generate a MPass logout response that can be used to initialize the process 
        /// of MPass logout. It should pe sent through POST method to http://mpass.gov.md/logout/saml
        /// with a field in formdata named SAMLRequest that has a base 64 signed login response xml 
        /// document
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="responseId"></param>
        /// <returns>A SAML login response</returns>
        public string GetMPassLogoutResponse(string responseId ,string requestId)
        {
            var logoutResponse = SamlMessage.BuildLogoutResponse(responseId, _mPassOptions.SAMLLogoutDestination, requestId, _mPassOptions.SAMLIssuer);
            logoutResponse = SamlMessage.Sign(logoutResponse, _mPassSigningCredentials.ServiceProviderCertificate);
            var dataBytes = Encoding.UTF8.GetBytes(logoutResponse);
            return Convert.ToBase64String(dataBytes);
        }
    }
}
