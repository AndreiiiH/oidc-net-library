using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;

namespace AndreiiiH.OIDC.Structural.ProviderConfigs
{
    public class OpenIdConfig
    {
        public OpenIdConnectConfiguration Config { get; set; }
        public string Nonce { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }

        internal string ResponseModeString { get; set; }
        private ResponseMode _responseModeEnum { get; set; }
        public ResponseMode ResponseMode
        {
            get { return _responseModeEnum; }
            set
            {
                _responseModeEnum = value;
                if (_responseModeEnum == ResponseMode.Query)
                {
                    ResponseModeString = "query";
                    return;
                }

                if (_responseModeEnum == ResponseMode.FormPost)
                {
                    ResponseModeString = "form_post";
                }
            }
        }

        public OpenIdConfig()
        {
            Nonce = Core.Nonce.Generate();
        }

        public OpenIdConfig ToOpenIdConfig()
        {
            OpenIdConfig config = new OpenIdConfig();
            
            config.Config = Config;
            config.Nonce = Nonce;
            config.ClientId = ClientId;
            config.ClientSecret = ClientSecret;
            config.RedirectUri = RedirectUri;
            config.ResponseMode = ResponseMode;

            return config;
        }

        public async Task GetConfig(string openIdConfigUrl)
        {
            ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(openIdConfigUrl);
            Config = await configManager.GetConfigurationAsync();
        }
    }

    public enum ResponseMode
    {
        Query, FormPost
    }
}
