using ChaoticPixel.OIDC.Structural.Flows;
using ChaoticPixel.OIDC.Structural.ProviderConfigs;

namespace ChaoticPixel.OIDC.Structural.Providers
{
    public class CustomProvider : BaseProvider
    {
        public CustomProvider(OpenIdConfig config, TokenCache tokenCache)
        {
            Config = config;
            TokenCache = tokenCache;
            
            AuthorizationCode = new AuthorizationCode();
            AuthorizationCode.OpenIdConfig = Config;
            AuthorizationCode.TokenCache = TokenCache;
        }
    }
}