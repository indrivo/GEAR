using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MobilpayEncryptDecrypt;

namespace GR.MobilPay.Extensions
{
    public static class EncryptDecryptExtensions
    {
        /// <summary>
        /// Encrypt with cng
        /// </summary>
        /// <param name="dummy"></param>
        /// <param name="mobilPayEncrypt"></param>
        /// <returns></returns>
        public static int EncryptWithCng(this MobilpayEncryptDecrypt.MobilpayEncryptDecrypt dummy, MobilpayEncrypt mobilPayEncrypt)
        {
            try
            {
                var bytes = Encoding.ASCII.GetBytes(mobilPayEncrypt.Data);
                var random = new Random();
                var array = new byte[8];
                for (var i = 0; i < array.Length; i++)
                {
                    array[i] = (byte)random.Next(0, 255);
                }
                Rc4(ref bytes, array);
                var x509Certificate = new X509Certificate2(mobilPayEncrypt.X509CertificateFilePath);

                var rSaCng = x509Certificate.GetRSAPublicKey();
                rSaCng.ExportParameters(false);
                var inArray = rSaCng.Encrypt(array, RSAEncryptionPadding.Pkcs1);
                mobilPayEncrypt.EncryptedData = Convert.ToBase64String(bytes);
                mobilPayEncrypt.EnvelopeKey = Convert.ToBase64String(inArray);
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
            return 0;
        }


        private static void Rc4(ref byte[] bytes, byte[] key)
        {
            var array = new byte[256];
            var array2 = new byte[256];
            int i;
            for (i = 0; i < 256; i++)
            {
                array[i] = (byte)i;
                array2[i] = key[i % key.GetLength(0)];
            }
            var num = 0;
            for (i = 0; i < 256; i++)
            {
                num = (num + array[i] + array2[i]) % 256;
                var b = array[i];
                array[i] = array[num];
                array[num] = b;
            }
            i = (num = 0);
            for (var j = 0; j < bytes.GetLength(0); j++)
            {
                i = (i + 1) % 256;
                num = (num + array[i]) % 256;
                var b = array[i];
                array[i] = array[num];
                array[num] = b;
                var num2 = (array[i] + array[num]) % 256;
                bytes[j] ^= array[num2];
            }
        }
    }
}
