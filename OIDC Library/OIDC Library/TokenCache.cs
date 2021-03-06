﻿using System;
using System.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Threading;

namespace AndreiiiH.OIDC
{
    public sealed class TokenCache
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private string _scopes;
        private string _authorizationCode;
        private JwtSecurityToken _idToken;
        private JwtSecurityToken _accessToken;
        private string _refreshToken;
        private AuthenticationHeaderValue _bearerHeader;

        private DateTime _validThru;

        public Guid Guid { get; }

        public OpenIdConnect Oidc { get; internal set; }

        public TokenCache()
        {
            Guid = Guid.NewGuid();
        }

        public void SetScopes(string scopes)
        {
            _lock.EnterWriteLock();
            _scopes = scopes;
            _lock.ExitWriteLock();
        }

        public string GetScopes()
        {
            _lock.EnterReadLock();
            string temp = _scopes;
            _lock.ExitReadLock();
            return temp;
        }

        public void SetAuthorizationCode(string authorizationCode)
        {
            _lock.EnterWriteLock();
            _authorizationCode = authorizationCode;
            _lock.ExitWriteLock();
        }

        public string GetAuthorizationCode()
        {
            _lock.EnterReadLock();
            string temp = _authorizationCode;
            _lock.ExitReadLock();
            return temp;
        }

        public void SetIdToken(JwtSecurityToken idToken)
        {
            _lock.EnterWriteLock();
            _idToken = idToken;
            _lock.ExitWriteLock();
        }

        public JwtSecurityToken GetIdToken()
        {
            _lock.EnterReadLock();
            JwtSecurityToken temp = _idToken;
            _lock.ExitReadLock();
            return temp;
        }

        public void SetAccessToken(JwtSecurityToken accessToken)
        {
            _lock.EnterWriteLock();
            _accessToken = accessToken;
            _lock.ExitWriteLock();
        }

        public JwtSecurityToken GetAccessToken()
        {
            _lock.EnterReadLock();
            JwtSecurityToken temp = _accessToken;
            _lock.ExitReadLock();
            return temp;
        }

        public void SetRefreshToken(string refreshToken)
        {
            _lock.EnterWriteLock();
            _refreshToken = refreshToken;
            _lock.ExitWriteLock();
        }

        public string GetRefreshToken()
        {
            _lock.EnterReadLock();
            string temp = _refreshToken;
            _lock.ExitReadLock();
            return temp;
        }

        public void SetValidThru(int offset)
        {
            _lock.EnterWriteLock();
            _validThru = DateTime.UtcNow.AddSeconds(offset);
            _lock.ExitWriteLock();
        }

        public DateTime GetValidThru()
        {
            _lock.EnterReadLock();
            DateTime temp = _validThru;
            _lock.ExitReadLock();
            return temp;
        }

        public AuthenticationHeaderValue GetBearerHeader()
        {
            if (_accessToken == null)
            {
                return null;
            }
            if (_bearerHeader == null)
            {
                _lock.EnterReadLock();
                _bearerHeader = new AuthenticationHeaderValue("Bearer", _accessToken.RawData);
                _lock.ExitReadLock();
            }
            return _bearerHeader;
        }
    }
}
