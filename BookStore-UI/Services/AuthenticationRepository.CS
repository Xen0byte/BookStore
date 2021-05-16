using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using UI.Contracts;
using UI.Models;
using UI.Providers;
using UI.Static;

namespace UI.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IHttpClientFactory _client;
        private readonly ILocalStorageService _localStorage;

        public AuthenticationRepository(IHttpClientFactory client, ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _client = client;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> LogIn(UserLoginModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.LoginEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var client = _client.CreateClient();
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponseModel>(content);

            await _localStorage.SetItemAsync("authToken", token?.Token);

            await ((APIAuthenticationStateProvider) _authenticationStateProvider).LoggedIn();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token?.Token);

            return true;
        }

        public async Task LogOut()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((APIAuthenticationStateProvider) _authenticationStateProvider).LoggedOut();
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