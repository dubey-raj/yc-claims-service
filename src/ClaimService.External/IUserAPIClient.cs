namespace ClaimService.External
{
    public interface IUserAPIClient
    {
        Task<TResponse?> GetAsync<TResponse>(string url);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data);
        Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data);
        Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest data);
        Task DeleteAsync(string url);
    }
}
