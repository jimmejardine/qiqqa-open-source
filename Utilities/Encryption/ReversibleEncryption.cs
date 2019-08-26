using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Utilities.Encryption
{
    public class ReversibleEncryption
    {
        // Change these keys 
        private byte[] Key = 
        { 
            184, 27,  162,  37,  12, 122, 209, 211, 
             24, 175, 154, 173,  53, 196,  39,  24,
             24, 175, 154, 173,  53, 196,  39,  24,
             26,  77, 218, 231, 236,  23, 209,  66
        };
        private byte[] Vector = { 146, 164, 91, 11, 123, 3, 13, 119, 235, 121, 25, 11, 179, 132, 114, 156 };


        private ICryptoTransform EncryptorTransform, DecryptorTransform;
        private UTF8Encoding UTFEncoder;

        
        public ReversibleEncryption()
        {
            RijndaelManaged rm = new RijndaelManaged();

            // Create an encryptor and a decryptor using our encryption method, key, and vector. 
            EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
            DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);

            // Used to translate bytes to text and vice versa 
            UTFEncoder = new UTF8Encoding();
        }

        /// ----------- The staticly used methods ------------------------------     

        public static ReversibleEncryption Instance = new ReversibleEncryption();

        public static string StaticEncryptString(string TextValue)
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (Instance)
            {
                l1_clk.LockPerfTimerStop();
                return Instance.EncryptString(TextValue);
            }
        }

        public static string StaticDecryptString(string TextValue)
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (Instance)
            {
                l1_clk.LockPerfTimerStop();
                return Instance.DecryptString(TextValue);
            }
        }

        /// ----------- The commonly used methods ------------------------------     
        

        
        /// Encrypt some text and return a string suitable for passing in a URL. 
        public string EncryptString(string TextValue)
        {
            if (null == TextValue) return null;
            return ByteArrToString(Encrypt(TextValue));
        }

        /// Encrypt some text and return an encrypted byte array. 
        public byte[] Encrypt(string TextValue)
        {
            // Translates our text value into a byte array. 
            Byte[] bytes = UTFEncoder.GetBytes(TextValue);

            // Used to stream the data in and out of the CryptoStream. 
            using (MemoryStream memoryStream = new MemoryStream())
            {
                /* 
                 * We will have to write the unencrypted bytes to the stream, 
                 * then read the encrypted result back from the stream. 
                 */
                #region Write the decrypted value to the encryption stream
                CryptoStream cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();
                #endregion

                #region Read encrypted value back out of the stream
                memoryStream.Position = 0;
                byte[] encrypted = new byte[memoryStream.Length];
                memoryStream.Read(encrypted, 0, encrypted.Length);
                #endregion

                //Clean up. 
                cs.Close();
                memoryStream.Close();

                return encrypted;
            }
        }

        /// The other side: Decryption methods 
        public string DecryptString(string EncryptedString)
        {
            if (String.IsNullOrEmpty(EncryptedString)) return null;
            return Decrypt(StrToByteArray(EncryptedString));
        }

        /// Decryption when working with byte arrays.     
        public string Decrypt(byte[] EncryptedValue)
        {
            #region Write the encrypted value to the decryption stream
            MemoryStream encryptedStream = new MemoryStream();
            CryptoStream decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
            decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
            decryptStream.FlushFinalBlock();
            #endregion

            #region Read the decrypted value from the stream.
            encryptedStream.Position = 0;
            Byte[] decryptedBytes = new Byte[encryptedStream.Length];
            encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
            encryptedStream.Close();
            #endregion
            return UTFEncoder.GetString(decryptedBytes);
        }

        /// Convert a string to a byte array.  NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so). 
        //      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding(); 
        //      return encoding.GetBytes(str); 
        // However, this results in character values that cannot be passed in a URL.  So, instead, I just 
        // lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100). 
        public byte[] StrToByteArray(string str)
        {
            if (str.Length == 0)
                throw new Exception("Invalid string value in StrToByteArray");

            byte val;
            byte[] byteArr = new byte[str.Length / 3];
            int i = 0;
            int j = 0;
            do
            {
                val = byte.Parse(str.Substring(i, 3));
                byteArr[j++] = val;
                i += 3;
            }
            while (i < str.Length);
            return byteArr;
        }

        // Same comment as above.  Normally the conversion would use an ASCII encoding in the other direction: 
        //      System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding(); 
        //      return enc.GetString(byteArr);     
        public string ByteArrToString(byte[] byteArr)
        {
            byte val;
            string tempStr = "";
            for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
            {
                val = byteArr[i];
                if (val < (byte)10)
                    tempStr += "00" + val.ToString();
                else if (val < (byte)100)
                    tempStr += "0" + val.ToString();
                else
                    tempStr += val.ToString();
            }
            return tempStr;
        }
    }
}