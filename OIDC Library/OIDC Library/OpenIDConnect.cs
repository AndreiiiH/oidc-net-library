using ChaoticPixel.OIDC.Core;
using Newtonsoft.Json.Linq;

namespace ChaoticPixel.OIDC
{
    public class OpenIDConnect
    {
        private OpenIDConnectConfig _openIdConfig;
        private TokenCache _tokenCache;
        private string _nonce;

        public string ConfigJSON { get { return _openIdConfig.ToString(); } }

        public OpenIDConnect(string azureTenant, string clientId, TokenCache tokenCache)
        {
            _openIdConfig = new OpenIDConnectConfig(azureTenant, clientId);
            _openIdConfig.ParseJObject(GetOIDCConfiguration(_openIdConfig.ConfigurationEndpoint));

            tokenCache.OIDC = this;
            _nonce = Nonce.Generate();
        }

        public string GetAuthorizationURL(ResponseType responseType, string redirectUri, ResponseMode responseMode, string scopes, string state, string nonce)
        {
            if (_tokenCache == null)
            {
                return string.Empty;
            }

            string url = string.Format("{0}?client_id={1}&response_type={2}&redirect_uri={3}&response_mode={4}&scope={5}&state={6}&nonce={7}",
                _openIdConfig.AuthorizationEndpoint, _openIdConfig.ParseResponseType(responseType), redirectUri, _openIdConfig.ParseResponseMode(responseMode),
                scopes, state, _nonce);

            return url;
        }

        public string GetLogoutUrl(string redirectUri)
        {
            string url = string.Format("{0}?post_logout_redirect_uri={1}", _openIdConfig.LogoutEndpoint, redirectUri);

            return url;
        }

        public bool ValidateToken(string token)
        {
            using (TokenValidator validator = new TokenValidator(token, _openIdConfig))
            {
                return validator.Validate(_nonce);
            }
        }

        private JObject GetOIDCConfiguration(string configurationEndpoint)
        {
            return HTTPRequest.GetWeb(configurationEndpoint);
        }
    }
}
