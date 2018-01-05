using System.IdentityModel.Tokens;
using ChaoticPixel.OIDC.Core;
using ChaoticPixel.OIDC.Structural.ProviderConfigs;
using ChaoticPixel.OIDC.Structural.Providers;

namespace ChaoticPixel.OIDC
{
    public class OpenIdConnect
    {
        private Providers _currentProviderType = Providers.Custom;
        private BaseProvider _currentProvider = null;

        public Providers ProviderType
        {
            get { return _currentProviderType; }
        }

        public BaseProvider Provider
        {
            get { return _currentProvider; }
        }
        
        public OpenIdConnect(OpenIdConfig openIdConfig, TokenCache tokenCache)
        {
            if (openIdConfig is MicrosoftOnlineConfig)
            {
                _currentProviderType = Providers.MicrosoftOnline;
            }

            switch (_currentProviderType)
            {
                case Providers.Custom:
                    _currentProvider = new CustomProvider(openIdConfig, tokenCache);
                    break;
                case Providers.MicrosoftOnline:
                    _currentProvider = new MicrosoftOnline((MicrosoftOnlineConfig)openIdConfig, tokenCache);
                    break;
                default:
                    _currentProvider = new CustomProvider(openIdConfig, tokenCache);
                    break;
            }
        }

        public JwtSecurityToken ValidateToken(string token)
        {
            using (TokenValidator validator = new TokenValidator(token, _currentProvider.Config.Config))
            {
                return validator.Validate(_currentProvider.Config.Nonce);
            }
        }
    }

    public enum Providers
    {
        Custom, MicrosoftOnline
    }
}
