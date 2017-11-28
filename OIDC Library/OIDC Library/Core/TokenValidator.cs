using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ChaoticPixel.OIDC.Core
{
    internal sealed class TokenValidator : IDisposable
    {
        private string _token;
        private string _header;
        private string _payload;
        private byte[] _signature;

        private string _algorithm;
        private string _privateKey;

        private OpenIDConnectConfig _config;

        public TokenValidator(string token, OpenIDConnectConfig config)
        {
            _token = token;
            string[] parts = _token.Split('.');

            _header = parts[0];
            _payload = parts[1];
            _signature = Base64UrlDecode(parts[2]);

            _config = config;
        }

        public bool Validate(string nonce)
        {
            return (ValidateNonce(nonce) && ValidateSignature());
        }

        private bool ValidateNonce(string nonce)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadToken(_token) as JwtSecurityToken;
            string tokenNonce = token.Claims.First(claim => claim.Type == "nonce").Value;
            if (nonce == tokenNonce)
            {
                return true;
            }
            return false;
        }

        private bool ValidateSignature()
        {
            JObject privateKeysJObject = HTTPRequest.GetWeb(_config.JWKSEndpoint);

            byte[] bytesToSign = Encoding.UTF8.GetBytes(string.Concat(_header, ".", _payload));
            _algorithm = (string)JObject.Parse(Encoding.UTF8.GetString(Base64UrlDecode(_header)))["alg"];
            string keyId = (string)JObject.Parse(Encoding.UTF8.GetString(Base64UrlDecode(_header)))["kid"];

            string key = privateKeysJObject.Values().First(token => token["kid"].ToString() == keyId)["n"].ToString();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            HMACSHA256 sha = new HMACSHA256(keyBytes);
            byte[] signature = sha.ComputeHash(bytesToSign);

            string decodedCrypto = Convert.ToBase64String(_signature);
            string decodedSignature = Convert.ToBase64String(signature);

            if (decodedCrypto == decodedSignature)
            {
                return true;
            }
            return false;
        }

        private static string Base64UrlEncode(byte[] input)
        {
            string output = Convert.ToBase64String(input);
            output = output.Split('=')[0];
            output = output.Replace('+', '-');
            output = output.Replace('/', '_');
            return output;
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string output = input;
            output = output.Replace('-', '+');
            output = output.Replace('_', '/');
            switch (output.Length % 4)
            {
                case 0:
                    break;
                case 2:
                    output += "==";
                    break;
                case 3:
                    output += "=";
                    break;
                default:
                    throw new Exception("Illegal Base64URL string!");
            }
            byte[] converted = Convert.FromBase64String(output);
            return converted;
        }

        private bool disposedValue = false;

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // No managed resources
                }

                _token = null;
                _header = null;
                _payload = null;
                _signature = null;
                _privateKey = null;
                _algorithm = null;
                _config = null;

                disposedValue = true;
            }
        }

        ~TokenValidator() {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
