using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UI.Contracts;

namespace UI.Services
{
    public class CRUDService<T> : IBaseRepository<T> where T : class
    {
        private readonly IHttpClientFactory _client;

        public CRUDService(IHttpClientFactory client)
        {
            _client = client;
        }

        public async Task<T> Get(string url, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url + id);

            var client = _client.CreateClient();
            var response = await client.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return null;
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<IList<T>> Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = _client.CreateClient();
            var response = await client.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return null;
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IList<T>>(content);
        }

        public async Task<bool> Create(string url, T obj)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            if (obj is null) return false;

            request.Content = new StringContent(JsonConvert.SerializeObject((obj)));

            var client = _client.CreateClient();
            var response = await client.SendAsync(request);

            return response.StatusCode == HttpStatusCode.Created;
        }

        public async Task<bool> Update(string url, T obj)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, url); /* https://restfulapi.net/rest-put-vs-post/ */

            if (obj is null) return false;

            request.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

            var client = _client.CreateClient();
            var response = await client.SendAsync(request);

            return response.StatusCode == HttpStatusCode.NoContent;
        }

        public async Task<bool> Delete(string url, int id)
        {
            if (id < 1) return false;

            var request = new HttpRequestMessage(HttpMethod.Delete, url + id);

            var client = _client.CreateClient();
            var response = await client.SendAsync(request);

            return response.StatusCode == HttpStatusCode.NoContent;
        }
    }
}