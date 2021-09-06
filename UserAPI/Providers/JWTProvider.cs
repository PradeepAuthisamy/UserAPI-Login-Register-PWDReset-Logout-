using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Providers.Interface;

namespace UserAPI.Providers
{
    public class JWTProvider : IJWTProvider
    {
        public async Task<string> GetTokenAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var token = string.Empty;
            using var authClient = new HttpClient();
            var cred = new
            {
                username = userName
            };

            string json = JsonConvert.SerializeObject(cred);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var tokenResponse = await authClient.PostAsync("http://jwt-authenticator:20/api/authenticator/authenticate", data);

            if (tokenResponse.IsSuccessStatusCode)
            {
                token = tokenResponse.Content.ReadAsStringAsync().Result;
            }

            return token;
        }
    }
}