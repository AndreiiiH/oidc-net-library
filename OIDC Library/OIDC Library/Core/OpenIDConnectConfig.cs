using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ChaoticPixel.OIDC.Core
{
    internal sealed class OpenIDConnectConfig
    {
        private string _azureTenant;
        private string _configurationEndpoint;
        private string _authorizationEndpoint;
        private string _tokenEndpoint;
        private string _jwksEndpoint;
        private string _logoutEndpoint;

        private string _clientId;

        public string AzureTenant { get { return _azureTenant; } }
        public string ConfigurationEndpoint { get { return _configurationEndpoint; } }
        public string AuthorizationEndpoint { get { return _authorizationEndpoint; } }
        public string TokenEndpoint { get { return _tokenEndpoint; } }
        public string JWKSEndpoint { get { return _jwksEndpoint; } }
        public string LogoutEndpoint { get { return _logoutEndpoint; } }

        public OpenIDConnectConfig(string azureTenant, string clientId)
        {
            _azureTenant = azureTenant;
            _configurationEndpoint = string.Format("https://login.microsoftonline.com/{0}/v2.0/.well-known/openid-configuration", _azureTenant);

            _clientId = clientId;
        }

        public void ParseJObject(JObject jObject)
        {
            _authorizationEndpoint = jObject["authorization_endpoint"].ToString();
            _tokenEndpoint = jObject["token_endpoint"].ToString();
            _jwksEndpoint = jObject["jwks_uri"].ToString();
            _logoutEndpoint = jObject["end_session_endpoint"].ToString();
        }

        public string ParseResponseType(ResponseType responseType)
        {
            return _responseTypeDictionary[responseType];
        }

        public string ParseResponseMode(ResponseMode responseMode)
        {
            return _responseModeDictionary[responseMode];
        }

        public override string ToString()
        {
            string output = string.Format("{{\n\t\"AzureTenant\": \"{0}\",\n\t\"ConfigurationEndpoint\": \"{1}\",\n\t\"AuthorizationEndpoint\": \"{2}\",\n\t\"TokenEndpoint\": \"{3}\",\n\t\"JWKSEndpoint\": \"{4}\",\n\t\"LogoutEndpoint\": \"{5}\"\n}}",
                AzureTenant, ConfigurationEndpoint, AuthorizationEndpoint, TokenEndpoint, JWKSEndpoint, LogoutEndpoint);
            return output;
        }

        private Dictionary<ResponseType, string> _responseTypeDictionary = new Dictionary<ResponseType, string>()
        {
            { ResponseType.Code, "code" },
            { ResponseType.IDToken, "id_token" },
            { ResponseType.Code_IDToken, "code id_token" },
            { ResponseType.IDToken_Token, "id_token token" }
        };

        private Dictionary<ResponseMode, string> _responseModeDictionary = new Dictionary<ResponseMode, string>()
        {
            { ResponseMode.Query, "query" },
            { ResponseMode.FormPost, "form_post" }
        };
    }

    public enum ResponseType
    {
        Code, IDToken, Code_IDToken, IDToken_Token
    }

    public enum ResponseMode
    {
        Query, FormPost
    }
}
