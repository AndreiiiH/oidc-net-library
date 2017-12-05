using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;

namespace ChaoticPixel.OIDC.Core
{
    public class OpenIdConfig
    {
        // public ConfigurationManager<OpenIdConnectConfiguration> ConfigManager { get; }
        public OpenIdConnectConfiguration Config { get; }
        public string Nonce { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }

        private OpenIdConfig(string clientId, string clientSecret, OpenIdConnectConfiguration config)
        {
            Config = config;

            ClientId = clientId;
            ClientSecret = clientSecret;

            Nonce = Core.Nonce.Generate();
        }

        public static async Task<OpenIdConfig> CreateConfig(string openIdConfigUrl, string clientId, string clientSecret)
        {
            ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(openIdConfigUrl);
            OpenIdConnectConfiguration config = await configManager.GetConfigurationAsync();

            return new OpenIdConfig(clientId, clientSecret, config);
        }
    }
}
