using ChaoticPixel.OIDC.Core;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace ChaoticPixel.OIDC
{
    public class OpenIDConnect
    {
        private ConfigurationManager<OpenIdConnectConfiguration> _configManager;
        private OpenIdConnectConfiguration _config;
        private TokenCache _tokenCache;
        private string _nonce;
        private string _clientId;

        public OpenIDConnect(string azureTenant, string clientId, TokenCache tokenCache)
        {
            _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(string.Format("https://login.microsoftonline.com/{0}/v2.0/.well-known/openid-configuration", azureTenant));

            _clientId = clientId;
            _tokenCache = tokenCache;
            tokenCache.OIDC = this;
            _nonce = Nonce.Generate();
        }

        public async Task Initialize()
        {
            _config = await _configManager.GetConfigurationAsync();
        }

        public string GetAuthorizationURL(string responseType, string redirectUri, string responseMode, string scopes, string state, string nonce)
        {
            if (_tokenCache == null)
            {
                return string.Empty;
            }

            string url = string.Format("{0}?client_id={1}&response_type={2}&redirect_uri={3}&response_mode={4}&scope={5}&state={6}&nonce={7}",
                _config.AuthorizationEndpoint, _clientId, responseType, redirectUri, responseMode, scopes, state, _nonce);

            return url;
        }

        public string GetLogoutUrl(string redirectUri)
        {
            string url = string.Format("{0}?post_logout_redirect_uri={1}", _config.EndSessionEndpoint, redirectUri);

            return url;
        }

        public JwtSecurityToken ValidateToken(string token)
        {
            using (TokenValidator validator = new TokenValidator(token, _config))
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
