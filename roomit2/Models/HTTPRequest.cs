using System.Net.Http;
using System.Text.Json;

namespace roomit2.Models
{
    public class HTTPRequest
    {
        public async Task<FriendlyCaptchaResponse> HTTPRequestAsync(string api_Key, string sitekey,string token, string url, HttpClient httpClient) 
        {
            FriendlyCaptchaResponse captchaResponse = new FriendlyCaptchaResponse();

            // neue Code HttpClient
            try
            {
                //var httpRequestMessage = new HttpRequestMessage()
                //{
                //    Method = HttpMethod.Post,
                //    RequestUri = new Uri("https://global.frcapi.com/api/v2/captcha/siteverify"),
                //};

                
                //httpClient.Timeout = TimeSpan.FromSeconds(40);
                //httpClient.DefaultRequestVersion = HttpVersion.Version20;
                //httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

                //httpClient.DefaultRequestHeaders.Add("X-API-Key", api_Key);

                var param = new Dictionary<string, string> { { "solution", token! }, { "secret", api_Key } };
                var parameters = new FormUrlEncodedContent(param);

                var response = await httpClient.PostAsync(url, parameters);
                var responseAsString = await response.Content.ReadAsStringAsync();
                captchaResponse = JsonSerializer.Deserialize<FriendlyCaptchaResponse>(responseAsString);

                return captchaResponse;
            }
            catch (Exception e)
            {
                return new FriendlyCaptchaResponse() { data = new Data() { challenge = new Challenge() { origin = e.Message } }, success = false  };
            }
        }
    }
}
