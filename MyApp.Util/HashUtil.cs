using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Util
{
    public class HashUtil
    {
        public const int SaltByteSize = 24;
        public const int _HashByteSize = 20; // to match the size of the PBKDF2-HMAC-SHA-1 hash 
        public const int Pbkdf2Iterations = 1000;
        public const int IterationIndex = 0;
        public const int SaltIndex = 1;
        public const int Pbkdf2Index = 2;

        public static string HashPassword(string password, byte[] salt=null!, int HashByteSize = _HashByteSize)
        {
            //var randomNumberGenerator = new RNGCryptoServiceProvider();
            if (salt == null)
            {
                salt = new byte[SaltByteSize];
                var randomNumberGenerator = RandomNumberGenerator.Create();
                randomNumberGenerator.GetBytes(salt);
            }
            

            var hash = GetPbkdf2Bytes(password, salt, Pbkdf2Iterations, HashByteSize);
            return Pbkdf2Iterations + ":" +
                   Convert.ToBase64String(salt) + ":" +
                   Convert.ToBase64String(hash);
        }

        public static string HashText(string password, byte[] salt = null!,int Iterations= Pbkdf2Iterations, int HashByteSize = _HashByteSize, bool PrefixIterationsAndSalt = false)
        {
            //var randomNumberGenerator = new RNGCryptoServiceProvider();
            if (salt == null)
            {
                salt = new byte[SaltByteSize];
                var randomNumberGenerator = RandomNumberGenerator.Create();
                randomNumberGenerator.GetBytes(salt);
            }
            var hash = GetPbkdf2Bytes(password, salt, Iterations, HashByteSize);
            return (PrefixIterationsAndSalt==true?(Pbkdf2Iterations + ":" +
                   Convert.ToBase64String(salt) + ":" ):null)+
                   Convert.ToBase64String(hash);
        }

        public static bool MatchSaltedHash(string textToMatch, string correctHashContainingSalt)
        {
            char[] delimiter = { ':' };
            var split = correctHashContainingSalt.Split(delimiter);
            var iterations = Int32.Parse(split[IterationIndex]);
            var salt = Convert.FromBase64String(split[SaltIndex]);
            var hash = Convert.FromBase64String(split[Pbkdf2Index]);

            var testHash = GetPbkdf2Bytes(textToMatch, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        //public static string HashLicenseMachineDetailContainingLicenseKey(string LicenseMachineDetailContainingKey)
        //{
        //    var randomNumberGenerator = new RNGCryptoServiceProvider();
        //    byte[] salt = Encoding.ASCII.GetBytes(DateTime.UtcNow.ToString("yyyyMMdd"));
            
        //    var hash = GetPbkdf2Bytes(LicenseMachineDetailContainingKey, salt, Pbkdf2Iterations, 512);
        //    return Convert.ToBase64String(hash);
        //}

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private static byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }
    }
}
