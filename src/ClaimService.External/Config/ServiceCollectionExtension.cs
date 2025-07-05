using ClaimService.External.AuthProvider;
using ClaimService.External.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClaimService.External.Config
{
    public static class ServiceCollectionExtension
    {
        private const string USER_SERVICE_NAME = "User";

        public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITokenProvider, TokenProvider>();
            services.AddSingleton<IUserAPIClient, UserAPIClient>();
            services.AddSingleton<IAPIClient, APIClient>();
            services.AddSingleton<IMemoryCacheService, MemoryCacheService>();

            return services;
        }

        public static IServiceCollection AddUserClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient(USER_SERVICE_NAME, client =>
            {
                var serviceConfig = configuration.GetSection($"API.Integrations:{USER_SERVICE_NAME}").Get<ServiceConfiguration>();
                if (serviceConfig == null)
                {
                    if (serviceConfig == null)
                    {
                        throw new ArgumentException($"No configuration found for key: {USER_SERVICE_NAME}");
                    }
                }
                client.BaseAddress = new Uri(serviceConfig.BaseUrl);
            })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                    };
                });

            return services;
        }
    }
}
