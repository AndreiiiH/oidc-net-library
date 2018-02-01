using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AndreiiiH.OIDC
{
    public static class HttpRequest
    {
        public async static Task<HttpResponseMessage> Post(string url, HttpContent httpContent)
        {
            using (HttpClient http = new HttpClient())
            {
                HttpResponseMessage response = await http.PostAsync(url, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                throw new InvalidDataException(response.StatusCode.ToString());
            }
        }
    }
}
