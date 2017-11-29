using ChaoticPixel.OIDC.Core;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Net.Http;
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
        private string _clientSecret;

        public OpenIDConnect(string azureTenant, string clientId, string clientSecret, TokenCache tokenCache)
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

        public async Task GetTokens(string scopes, string redirectUri)
        {
            Dictionary<string, string> requestContent = new Dictionary<string, string>()
            {
                { "client_id", _clientId },
                { "scope", scopes },
                { "code", _tokenCache.GetAuthorizationCode() },
                { "redirect_uri", redirectUri },
                { "grant_type", "authorization_code" },
                { "client_secret", _clientSecret }
            };
            HttpResponseMessage response = await HTTPRequest.Post(_config.TokenEndpoint, new FormUrlEncodedContent(requestContent));
            string responseJSON = await response.Content.ReadAsStringAsync();
            JObject responseJObject = JObject.Parse(responseJSON);

            _tokenCache.SetScopes(responseJObject["scope"].ToString());
            _tokenCache.SetValidThru(int.Parse(responseJObject["expires_in"].ToString()));
            _tokenCache.SetAccessToken(new JwtSecurityToken(responseJObject["access_token"].ToString()));
            _tokenCache.SetRefreshToken(new JwtSecurityToken(responseJObject["refresh_token"].ToString()));
        }

        public async Task RefreshToken(string redirectUri)
        {
            Dictionary<string, string> requestContent = new Dictionary<string, string>()
            {
                { "client_id", _clientId },
                { "scope", _tokenCache.GetScopes() },
                { "refresh_token", _tokenCache.GetRefreshToken().RawData },
                { "redirect_uri", redirectUri },
                { "grant_type", "refresh_token" },
                { "client_secret", _clientSecret }
            };

            HttpResponseMessage response = await HTTPRequest.Post(_config.TokenEndpoint, new FormUrlEncodedContent(requestContent));
            string responseJSON = await response.Content.ReadAsStringAsync();
            JObject responseJObject = JObject.Parse(responseJSON);

            _tokenCache.SetValidThru(int.Parse(responseJObject["expires_in"].ToString()));
            _tokenCache.SetAccessToken(new JwtSecurityToken(responseJObject["access_token"].ToString()));
            _tokenCache.SetRefreshToken(new JwtSecurityToken(responseJObject["refresh_token"].ToString()));
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
    }
}
