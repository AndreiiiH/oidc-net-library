using System.Threading.Tasks;
using AndreiiiH.OIDC.Core;

namespace AndreiiiH.OIDC.Structural.ProviderConfigs
{
    public class MicrosoftOnlineConfig : OpenIdConfig
    {
        public new Task GetConfig(string azureTenant = "common")
        {
            return base.GetConfig($"https://login.microsoftonline.com/{azureTenant}/v2.0/.well-known/openid-configuration");
        }
    }
}