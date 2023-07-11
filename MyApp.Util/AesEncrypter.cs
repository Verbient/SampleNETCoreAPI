using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Util
{

    /// <summary>
    /// This class can be used to Encrypt-Decrypt the AppSettings.json contents
    /// </summary>
    public static class AesEncrypter
    {
        //b14ca5898a4e4133bbce2ea2315a1916
        private static readonly string _key = "hutdb6s42ln^ds@ohtre8g543m8d2*n4";

        private static Dictionary<string, string> keyValues=null!; 
        public static string EncryptString(string plainText, string key32Bit = null!)
        {
            key32Bit = key32Bit?? _key ;
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key32Bit);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using MemoryStream memoryStream = new();
                using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
                using (StreamWriter streamWriter = new(cryptoStream))
                {
                    streamWriter.Write(plainText);
                }

                array = memoryStream.ToArray();
            }
            return Convert.ToBase64String(array);
        }

        // Encrypt and postfix a special symbol to identify encryption
        public static string CustomEncryptString(string plainText, string key32Bit = null!)
        {
            string crypt = EncryptString( plainText, key32Bit);
            if (!string.IsNullOrEmpty(crypt))
                crypt += "??";

            return crypt;
        }
        public static string DecryptString(string cipherText, string key32Bit = null!)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            key32Bit = (key32Bit == null ? _key : key32Bit);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key32Bit);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new (buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new (cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        static readonly object SpinLock = new ();
        public static string CustomDecryptString(string Text, string key32Bit = null!) 
        {
            string result = null!;
            if ((Text??"").EndsWith("??"))
            {
                lock (SpinLock)
                {
                    if (keyValues == null)
                    {
                        keyValues = new Dictionary<string, string>();
                    }
                    if (keyValues.ContainsKey(Text!))
                    {
                        result = keyValues[Text!];
                    }
                    else
                    {
                        result = DecryptString(Text!.Substring(0, Text.Length - 2),key32Bit);
                        keyValues[Text] = result;
                    }
                }
                return result;
            }
            return Text!;
        }
    }
}
