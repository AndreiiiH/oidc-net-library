using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Threading.Tasks;
using AndreiiiH.OIDC.Structural.ProviderConfigs;
using Newtonsoft.Json.Linq;

namespace AndreiiiH.OIDC.Structural.Flows
{
    public class BaseFlow
    {
        internal OpenIdConfig OpenIdConfig { get; set; }
        internal TokenCache TokenCache { get; set; }

        protected async Task PostToken(Dictionary<string, string> requestBody, TokenCache tokenCache)
        {
            HttpResponseMessage response = await HttpRequest.Post(OpenIdConfig.Config.TokenEndpoint, new FormUrlEncodedContent(requestBody));
            string responseJson = await response.Content.ReadAsStringAsync();
            JObject responseJObject = JObject.Parse(responseJson);

            TokenCache.SetValidThru(int.Parse(responseJObject["expires_in"].ToString()));
            TokenCache.SetAccessToken(new JwtSecurityToken(responseJObject["access_token"].ToString()));
            TokenCache.SetRefreshToken(responseJObject["refresh_token"].ToString());
        }
    }
}