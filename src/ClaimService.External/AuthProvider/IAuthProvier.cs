namespace ClaimService.External.AuthProvider
{
    public interface ITokenProvider
    {
        Task<string> GetTokenAsync(string key);
    }
}
