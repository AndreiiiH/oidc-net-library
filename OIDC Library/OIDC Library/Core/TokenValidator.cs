﻿using Microsoft.IdentityModel.Protocols;
using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;

namespace ChaoticPixel.OIDC.Core
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
            if (!ValidateNonce(nonce))
            {
                return null;
            }
            return ValidateSignature();
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

            SecurityToken jwt;
            ClaimsPrincipal principal = handler.ValidateToken(_token, validationParameters, out jwt);
            return jwt as JwtSecurityToken;
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
