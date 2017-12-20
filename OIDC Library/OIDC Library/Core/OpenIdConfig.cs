using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;

namespace ChaoticPixel.OIDC.Core
{
    public class OpenIdConfig
    {
        // public ConfigurationManager<OpenIdConnectConfiguration> ConfigManager { get; }
        public OpenIdConnectConfiguration Config { get; set; }
        public string Nonce { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }

        public OpenIdConfig(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;

            Nonce = Core.Nonce.Generate();
        }

        public async Task GetConfig(string openIdConfigUrl)
        {
            ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(openIdConfigUrl);
            Config = await configManager.GetConfigurationAsync();
        }
    }
}
