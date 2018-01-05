using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChaoticPixel.OIDC.Structural.Flows
{
    public class ClientCredentials : BaseFlow
    {
        public async Task GetToken()
        {
            Dictionary<string, string> requestContent = new Dictionary<string, string>()
            {
                { "client_id", OpenIdConfig.ClientId },
                { "client_secret", OpenIdConfig.ClientSecret },
                { "scope", "https://graph.microsoft.com/.default" },
                { "redirect_uri", OpenIdConfig.RedirectUri },
                { "grant_type", "client_credentials" }
            };
            HttpResponseMessage response = await HttpRequest.Post(OpenIdConfig.Config.TokenEndpoint, new FormUrlEncodedContent(requestContent));
            string responseJson = await response.Content.ReadAsStringAsync();
            JObject responseJObject = JObject.Parse(responseJson);

            TokenCache.SetValidThru(int.Parse(responseJObject["expires_in"].ToString()));
            TokenCache.SetAccessToken(new JwtSecurityToken(responseJObject["access_token"].ToString()));
        }
    }
}