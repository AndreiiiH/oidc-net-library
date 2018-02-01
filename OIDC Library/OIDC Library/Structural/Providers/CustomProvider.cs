using AndreiiiH.OIDC.Structural.Flows;
using AndreiiiH.OIDC.Structural.ProviderConfigs;

namespace AndreiiiH.OIDC.Structural.Providers
{
    public class CustomProvider : BaseProvider
    {
        public CustomProvider(OpenIdConfig config, TokenCache tokenCache) : base(config, tokenCache)
        {
        }
    }
}