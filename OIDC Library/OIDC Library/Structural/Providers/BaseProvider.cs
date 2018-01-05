using ChaoticPixel.OIDC.Structural.Flows;
using ChaoticPixel.OIDC.Structural.ProviderConfigs;

namespace ChaoticPixel.OIDC.Structural.Providers
{
    public class BaseProvider
    {
        public OpenIdConfig Config { get; set; }
        public TokenCache TokenCache { get; set; }
        
        public AuthorizationCode AuthorizationCode { get; set; }
    }
}