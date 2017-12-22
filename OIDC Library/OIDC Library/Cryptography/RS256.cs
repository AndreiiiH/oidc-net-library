using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ChaoticPixel.OIDC.Core;

namespace ChaoticPixel.OIDC.Cryptography
{
    public static class RS256
    {
        private static readonly string _password;
        private static readonly string _salt;

        static RS256()
        {
            _salt = GenerateRandomString(24);
            _password = GenerateRandomString(64);
        }

        public static string Encrypt(string data)
        {
            byte[] dataArray = Encoding.Unicode.GetBytes(data);
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(_password, Encoding.Unicode.GetBytes(_salt));

            byte[] encryptedData = Encrypt(dataArray, rfc.GetBytes(32), rfc.GetBytes(16));

            return Convert.ToBase64String(encryptedData);
        }

        public static string Encrypt(string data, string password)
        {
            byte[] dataArray = Encoding.Unicode.GetBytes(data);
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            byte[] encryptedData = Encrypt(dataArray, rfc.GetBytes(32), rfc.GetBytes(16));

            return Convert.ToBase64String(encryptedData);
        }

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] initVector)
        {
            using (Rijndael alg = Rijndael.Create())
            {
                alg.Key = key;
                alg.IV = initVector;
                alg.Mode = CipherMode.CBC;
                alg.Padding = PaddingMode.Zeros;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }
                    
                    return ms.ToArray();
                }
            }
        }

        public static string Decrypt(string data)
        {
            byte[] dataArray = Convert.FromBase64String(data.Replace(' ', '+'));
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(_password, Encoding.Unicode.GetBytes(_salt));

            byte[] decryptedData = Decrypt(dataArray, rfc.GetBytes(32), rfc.GetBytes(16));

            return Encoding.Unicode.GetString(decryptedData);
        }

        public static string Decrypt(string data, string password)
        {
            byte[] dataArray = Convert.FromBase64String(data.Replace(' ', '+'));
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

            byte[] decryptedData = Decrypt(dataArray, rfc.GetBytes(32), rfc.GetBytes(16));

            return Encoding.Unicode.GetString(decryptedData);
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] initVector)
        {
            using (Rijndael alg = Rijndael.Create())
            {
                alg.Key = key;
                alg.IV = initVector;
                alg.Mode = CipherMode.CBC;
                alg.Padding = PaddingMode.Zeros;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }

                    return ms.ToArray();
                }
            }
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()[]{}|;':,./<>?`~";
            char[] stringChars = new char[length];
            Random random = RandomProvider.GetThreadRandom();

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}
