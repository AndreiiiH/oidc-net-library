using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Threading.Tasks;
using AndreiiiH.OIDC.Structural.Scaffolding;
using Newtonsoft.Json.Linq;

namespace AndreiiiH.OIDC.Structural.Flows
{
    public class AuthorizationCode : BaseFlow, IBaseFlow, IUserFlows, IAuthCodeFlow
    {
        
        public async Task GetToken(string scope)
        {
            Dictionary<string, string> requestBody = new Dictionary<string, string>()
            {
                { "client_id", OpenIdConfig.ClientId },
                { "scope", scope },
                { "code", TokenCache.GetAuthorizationCode() },
                { "redirect_uri", OpenIdConfig.RedirectUri },
                { "grant_type", "authorization_code" },
                { "client_secret", OpenIdConfig.ClientSecret }
            };
            
            HttpResponseMessage response = await HttpRequest.Post(OpenIdConfig.Config.TokenEndpoint, new FormUrlEncodedContent(requestBody));
            string responseJson = await response.Content.ReadAsStringAsync();
            JObject responseJObject = JObject.Parse(responseJson);

            TokenCache.SetScopes(responseJObject["scope"].ToString());
            TokenCache.SetValidThru(int.Parse(responseJObject["expires_in"].ToString()));
            TokenCache.SetAccessToken(new JwtSecurityToken(responseJObject["access_token"].ToString()));
            TokenCache.SetRefreshToken(responseJObject["refresh_token"].ToString());
        }

        public async Task RefreshToken()
        {
            Dictionary<string, string> requestBody = new Dictionary<string, string>()
            {
                { "client_id", OpenIdConfig.ClientId },
                { "scope", TokenCache.GetScopes() },
                { "refresh_token", TokenCache.GetRefreshToken() },
                { "redirect_uri", OpenIdConfig.RedirectUri },
                { "grant_type", "refresh_token" },
                { "client_secret", OpenIdConfig.ClientSecret }
            };

            await PostToken(requestBody, TokenCache);
        }

        public string GetLogoutUrl(string postLogoutRedirectUrl)
        {
            string url = $"{OpenIdConfig.Config.EndSessionEndpoint}?post_logout_redirect_uri={postLogoutRedirectUrl}";

            return url;
        }

        public string GetAuthorizationUrl(string scope, string state)
        {
            if (TokenCache == null)
            {
                return string.Empty;
            }

            string url = $"{OpenIdConfig.Config.AuthorizationEndpoint}?client_id={OpenIdConfig.ClientId}&response_type=code id_token&redirect_uri={OpenIdConfig.RedirectUri}&response_mode={OpenIdConfig.ResponseModeString}&scope={scope}&state={state}&nonce={OpenIdConfig.Nonce}";

            return url;
        }
    }
}