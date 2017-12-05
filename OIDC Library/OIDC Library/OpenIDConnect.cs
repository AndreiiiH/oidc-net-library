using ChaoticPixel.OIDC.Core;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChaoticPixel.OIDC
{
    public class OpenIdConnect
    {
        private readonly OpenIdConfig _openIdConfig;
        private readonly TokenCache _tokenCache;

        public OpenIdConnect(OpenIdConfig openIdConfig, TokenCache tokenCache)
        {
            _openIdConfig = openIdConfig;
            _tokenCache = tokenCache;
            tokenCache.Oidc = this;
        }

        public string GetAuthorizationUrl(string responseType, string redirectUri, string responseMode, string scopes, string state)
        {
            if (_tokenCache == null)
            {
                return string.Empty;
            }

            string url = $"{_openIdConfig.Config.AuthorizationEndpoint}?client_id={_openIdConfig.ClientId}&response_type={responseType}&redirect_uri={redirectUri}&response_mode={responseMode}&scope={scopes}&state={state}&nonce={_openIdConfig.Nonce}";

            return url;
        }

        public async Task GetTokens(string scopes, string redirectUri)
        {
            Dictionary<string, string> requestContent = new Dictionary<string, string>()
            {
                { "client_id", _openIdConfig.ClientId },
                { "scope", scopes },
                { "code", _tokenCache.GetAuthorizationCode() },
                { "redirect_uri", redirectUri },
                { "grant_type", "authorization_code" },
                { "client_secret", _openIdConfig.ClientSecret }
            };
            HttpResponseMessage response = await HttpRequest.Post(_openIdConfig.Config.TokenEndpoint, new FormUrlEncodedContent(requestContent));
            string responseJson = await response.Content.ReadAsStringAsync();
            JObject responseJObject = JObject.Parse(responseJson);

            _tokenCache.SetScopes(responseJObject["scope"].ToString());
            _tokenCache.SetValidThru(int.Parse(responseJObject["expires_in"].ToString()));
            _tokenCache.SetAccessToken(new JwtSecurityToken(responseJObject["access_token"].ToString()));
            _tokenCache.SetRefreshToken(responseJObject["refresh_token"].ToString());
        }

        public async Task RefreshToken(string redirectUri)
        {
            Dictionary<string, string> requestContent = new Dictionary<string, string>()
            {
                { "client_id", _openIdConfig.ClientId },
                { "scope", _tokenCache.GetScopes() },
                { "refresh_token", _tokenCache.GetRefreshToken() },
                { "redirect_uri", redirectUri },
                { "grant_type", "refresh_token" },
                { "client_secret", _openIdConfig.ClientSecret }
            };

            HttpResponseMessage response = await HttpRequest.Post(_openIdConfig.Config.TokenEndpoint, new FormUrlEncodedContent(requestContent));
            string responseJson = await response.Content.ReadAsStringAsync();
            JObject responseJObject = JObject.Parse(responseJson);

            _tokenCache.SetValidThru(int.Parse(responseJObject["expires_in"].ToString()));
            _tokenCache.SetAccessToken(new JwtSecurityToken(responseJObject["access_token"].ToString()));
            _tokenCache.SetRefreshToken(responseJObject["refresh_token"].ToString());
        }

        public string GetLogoutUrl(string redirectUri)
        {
            string url = $"{_openIdConfig.Config.EndSessionEndpoint}?post_logout_redirect_uri={redirectUri}";

            return url;
        }

        public JwtSecurityToken ValidateToken(string token)
        {
            using (TokenValidator validator = new TokenValidator(token, _openIdConfig.Config))
            {
                return validator.Validate(_openIdConfig.Nonce);
            }
        }
    }
}
