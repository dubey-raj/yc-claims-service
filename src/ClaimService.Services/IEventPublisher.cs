namespace ClaimService.Services
{
    public interface IEventPublisher
    {
        Task<bool> PublishEventAsync<T>(T message);
    }
}
