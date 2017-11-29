using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ChaoticPixel.OIDC.Core
{
    public static class State
    {
        private static DateTime _created = DateTime.UtcNow;
        private const string _PASSWORD = "ChaoticPixel:State";
        private const string _SALT = "Q2hhb3RpY1BpeGVsOlN0YXRl";
        private const string _INIT_VECTOR = "OFRna74m*aze01xY";

        public static string Encrypt(string state)
        {
            string signature = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _created.ToString(), _SALT)));
            string formatted = string.Format("state_{0}:signature_{1}", state, signature);
            return Convert.ToBase64String(EncryptToBytes(formatted));
        }

        public static string Decrypt(string state)
        {
            byte[] encryptedState = Convert.FromBase64String(state.Replace(' ', '+'));
            return DecryptFromBytes(encryptedState).TrimEnd('\0');
        }

        public static string Validate(string state)
        {
            string[] formattedParts = state.Split(':');
            string[] stateParts = formattedParts[0].Split('_');
            string[] signatureParts = formattedParts[1].Split('_');
            string[] signatureSubParts = signatureParts[1].Split(':');

            if (DateTime.Parse(signatureSubParts[0]) != _created)
            {
                return string.Empty;
            }
            if (signatureSubParts[1] != _SALT)
            {
                return string.Empty;
            }
            return stateParts[1];
        }

        private static byte[] EncryptToBytes(string formattedState)
        {
            int keySize = 256;

            byte[] stateBytes = Encoding.ASCII.GetBytes(formattedState);
            byte[] initialVector = Encoding.ASCII.GetBytes(_INIT_VECTOR);
            byte[] salt = Encoding.ASCII.GetBytes(_SALT);
            byte[] key = new Rfc2898DeriveBytes(_PASSWORD, salt).GetBytes(keySize / 8);

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

        private static string DecryptFromBytes(byte[] encryptedState)
        {
            int keySize = 256;

            byte[] outputBytes = new byte[encryptedState.Length];
            byte[] salt = Encoding.ASCII.GetBytes(_SALT);
            byte[] initialVector = Encoding.ASCII.GetBytes(_INIT_VECTOR);
            byte[] key = new Rfc2898DeriveBytes(_PASSWORD, salt).GetBytes(keySize / 8);

            using (RijndaelManaged symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;

                using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(key, initialVector))
                {
                    using (MemoryStream memoryStream = new MemoryStream(encryptedState))
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
