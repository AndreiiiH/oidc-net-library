using System;
using System.Security.Cryptography;
using System.Text;

namespace ChaoticPixel.OIDC.Core
{
    public static class Nonce
    {
        private static Random _random = RandomProvider.GetThreadRandom();
        private static DateTime _created = DateTime.UtcNow;

        public static string Generate()
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(SHA1Encrypt(_created + _random.Next().ToString())));
        }

        private static string SHA1Encrypt(string input)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA1CryptoServiceProvider sha1Hasher = new SHA1CryptoServiceProvider();
            byte[] hashedDataBytes = sha1Hasher.ComputeHash(encoder.GetBytes(input));

            return ByteArrayToString(hashedDataBytes);
        }

        private static string ByteArrayToString(byte[] input)
        {
            StringBuilder output = new StringBuilder("");

            for (int i = 0; i < input.Length; i++)
            {
                output.Append(input[i].ToString("X2"));
            }

            return output.ToString();
        }
    }
}
