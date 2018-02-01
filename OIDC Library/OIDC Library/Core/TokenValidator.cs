using Microsoft.IdentityModel.Protocols;
using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;

namespace AndreiiiH.OIDC.Core
{
    internal sealed class TokenValidator : IDisposable
    {
        private string _token;

        private OpenIdConnectConfiguration _config;

        public TokenValidator(string token, OpenIdConnectConfiguration config)
        {
            _token = token;
            _config = config;
        }

        public JwtSecurityToken Validate(string nonce)
        {
            return !ValidateNonce(nonce) ? null : ValidateSignature();
        }

        private bool ValidateNonce(string nonce)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadToken(_token) as JwtSecurityToken;
            string tokenNonce = token?.Claims.First(claim => claim.Type == "nonce").Value;
            return nonce == tokenNonce;
        }

        private JwtSecurityToken ValidateSignature()
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningTokens = _config.SigningTokens,
                ValidateLifetime = true
            };

            ClaimsPrincipal principal = handler.ValidateToken(_token, validationParameters, out SecurityToken jwt);
            return jwt as JwtSecurityToken;
        }

        private bool disposedValue = false;

        private void Dispose(bool disposing)
        {
            if (disposedValue)
            {
                return;
            }
            
            if (disposing)
            {
                // No managed resources
            }

            _token = null;
            _config = null;

            disposedValue = true;
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
