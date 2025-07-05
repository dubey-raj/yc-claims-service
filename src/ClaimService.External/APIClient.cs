using ClaimService.External.AuthProvider;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ClaimService.External
{
    public class APIClient : IAPIClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public APIClient(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        private async Task<HttpClient> CreateClientWithTokenAsync(string key)
        {
            //Commented getting auth token for testing
            var token = await _tokenProvider.GetTokenAsync(key);
            //var token = await Task.FromResult("access_token");
            var client = _httpClientFactory.CreateClient(key);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }

        public async Task<TResponse?> GetAsync<TResponse>(string key, string endpoint)
        {
            var client = await CreateClientWithTokenAsync(key);
            var response = await client.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(responseData);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string key, string endpoint, TRequest data)
        {
            var client = await CreateClientWithTokenAsync(key);
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(responseData);
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string key, string endpoint, TRequest data)
        {
            var client = await CreateClientWithTokenAsync(key);
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(endpoint, content);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(responseData);
        }

        public async Task DeleteAsync(string key, string endpoint)
        {
            var client = await CreateClientWithTokenAsync(key);
            var response = await client.DeleteAsync(endpoint);

            response.EnsureSuccessStatusCode();
        }

        public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string key, string endpoint, TRequest data)
        {
            var client = await CreateClientWithTokenAsync(key);
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PatchAsync(endpoint, content);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(responseData);
        }
    }
}

