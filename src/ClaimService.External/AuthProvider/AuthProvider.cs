using ClaimService.External.Caching;
using ClaimService.External.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace ClaimService.External.AuthProvider
{
    public class TokenProvider : ITokenProvider
    {
        private readonly ILogger<TokenProvider> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly IConfiguration _configuration;

        public TokenProvider(ILogger<TokenProvider> logger, IHttpClientFactory httpClientFactory,
            IMemoryCacheService memoryCacheService, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _memoryCacheService = memoryCacheService;
            _configuration = configuration;
        }

        public async Task<string> GetTokenAsync(string key)
        {
            var config = _configuration.GetSection($"API.Integrations:{key}").Get<ServiceConfiguration>();

            if (config == null)
            {
                throw new ArgumentException($"No configuration found for key: {key}");
            }
            //cache auth token per client
            var cacheKey = $"{config.ClientId}";
            if (_memoryCacheService.TryGet<string>(cacheKey, out var cachedToken))
            {
                _logger.LogInformation("Found cached token using that");
                return cachedToken;
            }

            _logger.LogInformation("Requesting new token from auth server");
            var client = _httpClientFactory.CreateClient("auth");
            var data = new
            {
                clientId = config.ClientId,
                clientSecret = config.ClientSecret,
                grantType = "client_credentials"
            };

            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(config.TokenEndpoint, content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);

            if (authResponse?.AccessToken is not null)
            {
                _memoryCacheService.Set(cacheKey, authResponse.AccessToken, TimeSpan.FromMinutes(55));
                _logger.LogInformation("New auth token recieved from auth server, it is cached");
                return authResponse.AccessToken;
            }

            throw new Exception("Failed to retrieve access token.");
        }
    }

}