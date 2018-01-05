using ChaoticPixel.OIDC.Structural.ProviderConfigs;

namespace ChaoticPixel.OIDC.Structural.Flows
{
    public class BaseFlow
    {
        internal OpenIdConfig OpenIdConfig { get; set; }
        internal TokenCache TokenCache { get; set; }
    }
}