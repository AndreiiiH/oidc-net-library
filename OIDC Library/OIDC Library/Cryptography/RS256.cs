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
        private static readonly string _initVector;

        static RS256()
        {
            _initVector = GenerateRandomString(16);
            _salt = GenerateRandomString(24);
            _password = GenerateRandomString(64);
        }

        public static string Encrypt(string input)
        {
            return Convert.ToBase64String(EncryptToBytes(input));
        }

        public static string Encrypt(string input, string password)
        {
            return Encoding.ASCII.GetString(EncryptToBytesPassword(input, password));
        }

        public static string Decrypt(string input)
        {
            byte[] encryptedState = Convert.FromBase64String(input.Replace(' ', '+'));
            return DecryptFromBytes(encryptedState).TrimEnd('\0');
        }

        public static string Decrypt(string input, string password)
        {
            return DecryptFromBytesPassword(Encoding.ASCII.GetBytes(input), password);
        }

        private static byte[] EncryptToBytesPassword(string input, string password)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] output;

            SymmetricAlgorithm symmetricAlgorithm = DES.Create();
            symmetricAlgorithm.Key = Encoding.ASCII.GetBytes(password);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(),
                    CryptoStreamMode.Write))
                {
                    cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                }

                output = memoryStream.ToArray();
            }

            return output;
        }

        private static byte[] EncryptToBytes(string input)
        {
            int keySize = 256;

            byte[] stateBytes = Encoding.ASCII.GetBytes(input);
            byte[] initialVector = Encoding.ASCII.GetBytes(_initVector);
            byte[] salt = Encoding.ASCII.GetBytes(_salt);
            byte[] key = new Rfc2898DeriveBytes(_password, salt).GetBytes(keySize / 8);

            using (RijndaelManaged symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(key, initialVector))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(stateBytes, 0, stateBytes.Length);
                            cryptoStream.FlushFinalBlock();

                            return memoryStream.ToArray();
                        }
                    }
                }
            }
        }

        private static string DecryptFromBytesPassword(byte[] input, string password)
        {
            string result;

            SymmetricAlgorithm symmetricAlgorithm = DES.Create();
            symmetricAlgorithm.Key = Encoding.ASCII.GetBytes(password);

            using (MemoryStream memoryStream = new MemoryStream(input))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(),
                    CryptoStreamMode.Read))
                {
                    byte[] decrypted = new byte[input.Length];
                    cryptoStream.Read(decrypted, 0, decrypted.Length);
                    result = Encoding.ASCII.GetString(decrypted);
                }
            }

            return result;
        }

        private static string DecryptFromBytes(byte[] input)
        {
            int keySize = 256;

            byte[] outputBytes = new byte[input.Length];
            byte[] salt = Encoding.ASCII.GetBytes(_salt);
            byte[] initialVector = Encoding.ASCII.GetBytes(_initVector);
            byte[] key = new Rfc2898DeriveBytes(_password, salt).GetBytes(keySize / 8);

            using (RijndaelManaged symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;

                using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(key, initialVector))
                {
                    using (MemoryStream memoryStream = new MemoryStream(input))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            int byteCount = cryptoStream.Read(outputBytes, 0, outputBytes.Length);

                            return Encoding.ASCII.GetString(outputBytes, 0, byteCount);
                        }
                    }
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
