using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace ChaoticPixel.OIDC
{
    public static class HTTPRequest
    {
        public static JObject GetWeb(string url)
        {
            using (WebClient web = new WebClient())
            {
                JObject responseJObject = null;

                try
                {
                    using (Stream stream = web.OpenRead(url))
                    {
                        StreamReader reader = new StreamReader(stream);
                        string response = reader.ReadToEnd();
                        responseJObject = JObject.Parse(response);
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Response is HttpWebResponse)
                    {
                        switch (((HttpWebResponse)ex.Response).StatusCode)
                        {
                            case HttpStatusCode.NotFound:
                                responseJObject = null;
                                break;
                            default:
                                throw ex;
                        }
                    }
                }

                return responseJObject;
            }
        }
    }
}
