using System;
using System.Net.Http.Headers;
using System.Threading;

namespace ChaoticPixel.OIDC
{
    public sealed class TokenCache
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private Guid _cacheGuid;
        private string _authorizationCode;
        private string _accessToken;
        private string _refreshToken;
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

        public void SetAccessToken(string accessToken)
        {
            _lock.EnterWriteLock();
            _accessToken = accessToken;
            _lock.ExitWriteLock();
        }

        public string GetAccessToken()
        {
            _lock.EnterReadLock();
            string temp = _accessToken;
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

        public AuthenticationHeaderValue GetBearerHeader()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                return null;
            }
            if (_bearerHeader == null)
            {
                _lock.EnterReadLock();
                _bearerHeader = new AuthenticationHeaderValue("Bearer", _accessToken);
                _lock.ExitReadLock();
            }
            return _bearerHeader;
        }
    }
}
