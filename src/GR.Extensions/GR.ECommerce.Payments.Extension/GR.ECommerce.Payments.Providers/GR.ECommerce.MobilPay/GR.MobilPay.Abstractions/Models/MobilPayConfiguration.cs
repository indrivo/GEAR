namespace GR.MobilPay.Abstractions.Models
{
    public sealed class MobilPayConfiguration
    {
        /// <summary>
        /// Is testing mode
        /// </summary>
        public bool IsSandbox { get; set; }

        /// <summary>
        /// Service url
        /// </summary>
        public string MobilPayUrl => IsSandbox ? "http://sandboxsecure.mobilpay.ro" : "https://secure.mobilpay.ro/";

        /// <summary>
        /// Signature
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// path to private key
        /// </summary>
        public string PathToPrivateKey { get; set; }

        /// <summary>
        /// Path to public key
        /// </summary>
        public string PathToCertificate { get; set; }
    }
}
