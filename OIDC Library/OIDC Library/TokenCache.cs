using System;
using System.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Threading;

namespace ChaoticPixel.OIDC
{
    public sealed class TokenCache
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private Guid _cacheGuid;
        private string _authorizationCode;
        private JwtSecurityToken _idToken;
        private JwtSecurityToken _accessToken;
        private JwtSecurityToken _refreshToken;
        private AuthenticationHeaderValue _bearerHeader;

        private OpenIDConnect _oidc;

        public Guid GUID { get; }

        public OpenIDConnect OIDC
        {
            get
            {
                return _oidc;
            }
            internal set
            {
                _oidc = value;
            }
        }

        public TokenCache()
        {
            _cacheGuid = Guid.NewGuid();
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

        public void SetIDToken(JwtSecurityToken idToken)
        {
            _lock.EnterWriteLock();
            _idToken = idToken;
            _lock.ExitWriteLock();
        }

        public JwtSecurityToken GetIDToken()
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

        public void SetRefreshToken(JwtSecurityToken refreshToken)
        {
            _lock.EnterWriteLock();
            _refreshToken = refreshToken;
            _lock.ExitWriteLock();
        }

        public JwtSecurityToken GetRefreshToken()
        {
            _lock.EnterReadLock();
            JwtSecurityToken temp = _refreshToken;
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
