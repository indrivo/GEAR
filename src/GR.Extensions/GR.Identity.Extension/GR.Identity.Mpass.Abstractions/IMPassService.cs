using System;
using System.Text;

namespace GR.Identity.Mpass.Abstractions
{
    /// <summary>
    /// Service that will generate MPass (http://mpass.gov.md) login requests
    /// and process responses.
    /// </summary>
    public interface IMPassService
    {
        /// <summary>
        /// Generate a MPass login request that can be used to initialize the process 
        /// of MPass Login. It should pe sent through POST method to http://mpass.gov.md/login/saml
        /// with a field in formdata named SAMLRequest that has a base 64 signed login request xml 
        /// document
        /// </summary>
        /// <returns>A SAML login request</returns>
        string GetMPassRequest(string requestId);

        /// <summary>
        /// Generate a MPass logout request that can be used to initialize the process 
        /// of MPass logout. It should pe sent through POST method to http://mpass.gov.md/logout/saml
        /// with a field in formdata named SAMLRequest that has a base 64 signed login request xml 
        /// document
        /// </summary>
        /// <param name="nameID"></param>
        /// <param name="sessionIndex"></param>
        /// <param name="requestId"></param>
        /// <returns>A SAML login request</returns>
        string GetMPassLogoutRequest(string requestId,string nameID, string sessionIndex);

        /// <summary>
        /// Generate a MPass logout response that can be used to initialize the process 
        /// of MPass logout. It should pe sent through POST method to http://mpass.gov.md/logout/saml
        /// with a field in formdata named SAMLRequest that has a base 64 signed login response xml 
        /// document
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="responseId"></param>
        /// <returns>A SAML login response</returns>
        string GetMPassLogoutResponse(string responseId,string requestID);

        /// <summary>
        /// Parse and retrieve <see cref="MPassUserInfo"/> from 
        /// the login response
        /// </summary>
        /// <param name="samlResponse">Response recieved after requesting a login</param>
        /// <returns><see cref="MPassUserInfo"/> object containing all the data or null if information retrieval is unsuccessful</returns>
        /// <exception cref="FormatException">May be thrown by <see cref="Convert.ToBase64String(byte[])"/></exception>
        /// <exception cref="DecoderFallbackException">May be thrown by <see cref="Encoding.UTF8"/></exception>
        MPassUserInfo GetMPassUserInfoFromLoginResponse(string samlResponse);
    }
}
