namespace ChaoticPixel.OIDC
{
    public class OpenIDConnect
    {
        private string _openIdConfigurationUri;

        public OpenIDConnect(string azureTenant)
        {
            _openIdConfigurationUri = string.Format("https://login.microsoftonline.com/{0}/v2.0/.well-known/openid-configuration", azureTenant);
        }
    }
}
