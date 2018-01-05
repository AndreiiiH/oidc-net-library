using System.Threading.Tasks;
using ChaoticPixel.OIDC.Core;

namespace ChaoticPixel.OIDC.Structural.ProviderConfigs
{
    public class MicrosoftOnlineConfig : OpenIdConfig
    {
        public Task GetConfig(string azureTenant = "common")
        {
            return base.GetConfig($"https://login.microsoftonline.com/{azureTenant}/v2.0/.well-known/openid-configuration");
        }
    }
}