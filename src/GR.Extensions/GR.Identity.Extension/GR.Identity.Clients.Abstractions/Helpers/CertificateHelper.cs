using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace GR.Identity.Clients.Abstractions.Helpers
{
    public class CertificateHelper
    {
        /// <summary>
        /// Get certificate
        /// </summary>
        /// <param name="manifestResourcePath"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static X509Certificate2 Get(string manifestResourcePath = "GR.Identity.Clients.Abstractions.Certificates.idsrv3test.pfx", string password = "idsrv3test")
        {
            var assembly = typeof(CertificateHelper).GetTypeInfo().Assembly;
            //var names = assembly.GetManifestResourceNames();

            /***********************************************************************************************
             *  Please note that here we are using a local certificate only for testing purposes. In a 
             *  real environment the certificate should be created and stored in a secure way, which is out
             *  of the scope of this project.
             **********************************************************************************************/
            using (var stream = assembly.GetManifestResourceStream(manifestResourcePath))
            {
                return new X509Certificate2(ReadStream(stream), password);
            }
        }

        /// <summary>
        /// Read stream
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static byte[] ReadStream(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
