using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AndreiiiH.OIDC.Cryptography;

namespace AndreiiiH.OIDC.Core
{
    public static class State
    {
        private static readonly string _signature;

        static State()
        {
            _signature = RS256.GenerateRandomString(16);
        }

        public static string Encrypt(string input)
        {
            string formatted = $"{input}${_signature}";
            return RS256.Encrypt(formatted);
        }

        public static string Decrypt(string input)
        {
            return RS256.Decrypt(input);
        }

        public static string Validate(string state)
        {
            string[] stateParts = state.Split('$');
            int similarity = String.Compare(_signature, stateParts[1], StringComparison.CurrentCulture);
            
            return similarity == 0 ? stateParts[0] : string.Empty;
        }
    }
}
