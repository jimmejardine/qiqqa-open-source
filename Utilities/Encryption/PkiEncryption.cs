using System;
using System.Security.Cryptography;
using System.Text;

namespace Utilities.Encryption
{
    public static class PkiEncryption
    {
        // Generate a new key pair
        public static void GenerateKeys(out string publicKey, out string privateKey)
        {
            // Variables
            CspParameters cspParams = null;

            // Create a new key pair on target CSP
            cspParams = new CspParameters();
            cspParams.ProviderType = 1; // PROV_RSA_FULL 
            //cspParams.ProviderName; // CSP name
            cspParams.Flags = CspProviderFlags.UseArchivableKey;
            cspParams.KeyNumber = (int)KeyNumber.Exchange;
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams))
            {
                // Export public key
                publicKey = rsaProvider.ToXmlString(false);

                // Export private/public key pair 
                privateKey = rsaProvider.ToXmlString(true);
            }
        }

        // Keys


        public static string Encrypt(string privateKeyText, string plainText)
        {
            // Variables
            CspParameters cspParams = null;

            byte[] plainBytes = null;
            byte[] encryptedBytes = null;

            // Select target CSP
            cspParams = new CspParameters();
            cspParams.ProviderType = 1; // PROV_RSA_FULL 
            //cspParams.ProviderName; // CSP name
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams))
            {
                // Import public key
                rsaProvider.FromXmlString(privateKeyText);

                // Encrypt plain text
                plainBytes = Encoding.Unicode.GetBytes(plainText);
                encryptedBytes = rsaProvider.Encrypt(plainBytes, false);
            }

            return Convert.ToBase64String(encryptedBytes);
        }


        public static string Decrypt(string publicKeyText, string base64EncryptedText)
        {

            // Variables
            CspParameters cspParams = null;
            string plainText = "";
            byte[] encryptedBytes = null;
            byte[] plainBytes = null;


            // Select target CSP
            cspParams = new CspParameters();
            cspParams.ProviderType = 1; // PROV_RSA_FULL 
            //cspParams.ProviderName; // CSP name
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams))
            {
                // Import private/public key pair
                rsaProvider.FromXmlString(publicKeyText);

                encryptedBytes = Convert.FromBase64String(base64EncryptedText);

                // Decrypt text
                plainBytes = rsaProvider.Decrypt(encryptedBytes, false);
            }

            plainText = Encoding.Unicode.GetString(plainBytes);

            return plainText;
        }

    }
}
