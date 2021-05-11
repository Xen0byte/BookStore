using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UI.Contracts;
using UI.Models;
using UI.Static;

namespace UI.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory _client;

        public AuthenticationRepository(IHttpClientFactory client)
        {
            _client = client;
        }

        public async Task<bool> LogIn(UserLoginModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.LoginEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var client = _client.CreateClient();
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            var token =  JsonConvert.DeserializeObject<TokenResponseModel>(content);

            // TODO: Store Token

            // TODO: Change Authentication State Of The Application

            return true;
        }

        public Task LogOut()
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Register(UserRegistrationModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.RegistrationEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var client = _client.CreateClient();
            var response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}