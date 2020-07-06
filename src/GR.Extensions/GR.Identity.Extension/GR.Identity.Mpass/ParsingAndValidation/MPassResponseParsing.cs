using System.Xml;
using GR.Identity.Mpass.Abstractions;
// ReSharper disable PossibleNullReferenceException

namespace GR.Identity.Mpass.ParsingAndValidation
{
    /// <summary>
    /// This class offers functionality of 
    /// extracting MPass User info from the 
    /// SAML Login Response.
    /// </summary>
    internal class MPassResponseParsing
    {
        private static XmlNamespaceManager _manager;

        private const string SuccessStatusCode = "urn:oasis:names:tc:SAML:2.0:status:Success";
        /// <summary>
        /// Get a <see cref="XmlNamespaceManager"/> with two namespaces required
        /// when parsing saml responses.
        /// </summary>
        /// <param name="xdoc">The <see cref="XmlDocument"/> that containts the SAML Response or an empty document that will be used for saml response</param>
        /// <returns><see cref="XmlNamespaceManager"/> object required for processing saml responses</returns>
        private static XmlNamespaceManager GetSamlnsManager(XmlDocument xdoc)
        {
            if (_manager == null)
            {
                _manager = new XmlNamespaceManager(xdoc.NameTable);

                _manager.AddNamespace("saml2p", "urn:oasis:names:tc:SAML:2.0:protocol");
                _manager.AddNamespace("saml2", "urn:oasis:names:tc:SAML:2.0:assertion");
            }
            return _manager;
        }

        /// <summary>
        /// Parse MPass SAML Response and get <see cref="MPassUserInfo"/> data 
        /// from it.
        /// </summary>
        /// <param name="xml">Xml String of the received MPass response</param>
        /// <returns><see cref="MPassUserInfo"/> object containing all the data</returns>
        public static MPassUserInfo GetMPassUserFromXml(string xml)
        {
            var xdoc = new XmlDocument();
            var ns = GetSamlnsManager(xdoc);

            xdoc.LoadXml(xml);
            var responseNode = xdoc.DocumentElement.SelectSingleNode("/saml2p:Response", ns);
            var statusNode = responseNode.SelectSingleNode("saml2p:Status", ns);
            var statusCodeNode = statusNode.SelectSingleNode("saml2p:StatusCode", ns);
            var statusCode = statusCodeNode.Attributes["Value"].Value;

            if (statusCode != SuccessStatusCode)
                return null;

            var assertionNode = responseNode.SelectSingleNode("saml2:Assertion", ns);
            var subjectNode = assertionNode.SelectSingleNode("saml2:Subject", ns);

            var userInfo = new MPassUserInfo
            {
                NameID = subjectNode.SelectSingleNode("saml2:NameID", ns).InnerText,
                RequestId = xdoc.DocumentElement.Attributes["InResponseTo"].Value
            };
            foreach (XmlNode personTag in assertionNode.SelectSingleNode("saml2:AttributeStatement",ns).ChildNodes)
            {

                switch (personTag.Attributes["Name"].Value.ToLower())
                {
                    case "firstname":
                        userInfo.FirstName = personTag.InnerText;
                        break;
                    case "lastname":
                        userInfo.LastName = personTag.InnerText;
                        break;
                    case "phone":
                        userInfo.PhoneNumber = personTag.InnerText;
                        break;
                    case "email":
                        userInfo.Email = personTag.InnerText;
                        break;
                    case "language":
                        userInfo.LangID = personTag.InnerText;
                        break;
                    case "gender":
                        userInfo.Gender = personTag.InnerText;
                        break;
                }
            }
            return userInfo;



        }
    }
}
