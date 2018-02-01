using AndreiiiH.OIDC.Structural.Flows;
using AndreiiiH.OIDC.Structural.ProviderConfigs;
using AndreiiiH.OIDC.Core;

namespace AndreiiiH.OIDC.Structural.Providers
{
    public class MicrosoftOnline : BaseProvider
    {
        public MicrosoftOnline(MicrosoftOnlineConfig config, TokenCache tokenCache) : base(config, tokenCache)
        {
        }
    }
}