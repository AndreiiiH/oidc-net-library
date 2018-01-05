using ChaoticPixel.OIDC.Core;
using ChaoticPixel.OIDC.Structural.Flows;
using ChaoticPixel.OIDC.Structural.ProviderConfigs;

namespace ChaoticPixel.OIDC.Structural.Providers
{
    public class MicrosoftOnline : BaseProvider
    {
        public MicrosoftOnline(MicrosoftOnlineConfig config, TokenCache tokenCache)
        {
            Config = config;
            TokenCache = tokenCache;
            
            AuthorizationCode = new AuthorizationCode();
            AuthorizationCode.OpenIdConfig = Config;
            AuthorizationCode.TokenCache = TokenCache;
        }
    }
}