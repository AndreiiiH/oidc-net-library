using AndreiiiH.OIDC.Structural.Flows;
using AndreiiiH.OIDC.Structural.ProviderConfigs;

namespace AndreiiiH.OIDC.Structural.Providers
{
    public class BaseProvider
    {
        public OpenIdConfig Config { get; set; }
        public TokenCache TokenCache { get; set; }
        
        public AuthorizationCode AuthorizationCode { get; set; }
        public ClientCredentials ClientCredentials { get; set; }

        public BaseProvider(OpenIdConfig config, TokenCache tokenCache)
        {
            Config = config;
            TokenCache = tokenCache;

            AuthorizationCode = new AuthorizationCode
            {
                OpenIdConfig = Config,
                TokenCache = TokenCache
            };

            ClientCredentials = new ClientCredentials
            {
                OpenIdConfig = Config,
                TokenCache = TokenCache
            };
        }
    }
}