namespace ClaimService.External
{
    public interface IAPIClient
    {
        Task<TResponse?> GetAsync<TResponse>(string key, string endpoint);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string key, string endpoint, TRequest data);
        Task<TResponse?> PutAsync<TRequest, TResponse>(string key, string endpoint, TRequest data);
        Task<TResponse?> PatchAsync<TRequest, TResponse>(string key, string endpoint, TRequest data);
        Task DeleteAsync(string key, string endpoint);
    }
}
