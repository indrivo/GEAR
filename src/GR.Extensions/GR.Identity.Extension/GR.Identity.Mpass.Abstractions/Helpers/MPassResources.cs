using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace GR.Identity.Mpass.Abstractions.Helpers
{
    public static class MPassResources
    {
        public static X509Certificate2 GetSandboxServiceProviderCertificate()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Certificates/samplempass.pfx");
            return new X509Certificate2(path, "qN6n31IT86684JO");
        }

        public static X509Certificate2 GetSandboxIdentityProviderCertificate()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Certificates/testmpass.cer");
            return new X509Certificate2(path);
        }
    }
}
