using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AndreiiiH.OIDC.Structural.Flows
{
    public class ClientCredentials : BaseFlow
    {
        public async Task GetToken()
        {
            Dictionary<string, string> requestBody = new Dictionary<string, string>()
            {
                { "client_id", OpenIdConfig.ClientId },
                { "client_secret", OpenIdConfig.ClientSecret },
                { "scope", "https://graph.microsoft.com/.default" },
                { "redirect_uri", OpenIdConfig.RedirectUri },
                { "grant_type", "client_credentials" }
            };

            await PostToken(requestBody, TokenCache);
        }
    }
}