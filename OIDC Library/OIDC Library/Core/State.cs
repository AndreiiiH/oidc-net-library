using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ChaoticPixel.OIDC.Cryptography;

namespace ChaoticPixel.OIDC.Core
{
    public static class State
    {
        private static readonly DateTime _created = DateTime.UtcNow;
        private static readonly string _signature;

        static State()
        {
            string salt = RS256.GenerateRandomString(16);
            _signature = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_created.ToString(CultureInfo.CurrentCulture)}:{salt}"));
        }

        public static string Encrypt(string input)
        {
            string formatted = $"state_{input}:signature_{_signature}";
            return RS256.Encrypt(formatted);
        }

        public static string Decrypt(string input)
        {
            return RS256.Decrypt(input);
        }

        public static string Validate(string state)
        {
            string[] formattedParts = state.Split(':');
            string[] stateParts = formattedParts[0].Split('_');
            string[] signatureParts = formattedParts[1].Split('_');

            if (signatureParts[1] != _signature)
            {
                return string.Empty;
            }
            return stateParts[1];
        }
    }
}
