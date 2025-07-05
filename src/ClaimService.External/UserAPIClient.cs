using Microsoft.Extensions.Logging;

namespace ClaimService.External
{
    public class UserAPIClient : IUserAPIClient
    {
        ILogger<UserAPIClient> _logger;
        private readonly IAPIClient _apiClient;
        private const string API_NAME = "User";

        public UserAPIClient(ILogger<UserAPIClient> logger, IAPIClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<TResponse?> GetAsync<TResponse>(string url)
        {
            var response = await _apiClient.GetAsync<TResponse>(API_NAME, url);
            return response;
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await _apiClient.PostAsync<TRequest, TResponse>(API_NAME, url, data);
            return response;
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await _apiClient.PutAsync<TRequest, TResponse>(API_NAME, url, data);
            return response;
        }

        public async Task DeleteAsync(string url)
        {
            await _apiClient.DeleteAsync(API_NAME, url);
        }

        public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await _apiClient.PatchAsync<TRequest, TResponse>(API_NAME, url, data);
            return response;
        }
    }
}

