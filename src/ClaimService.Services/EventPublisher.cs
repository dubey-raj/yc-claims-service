using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ClaimService.Services
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly string _queueUrl;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(ILogger<EventPublisher> logger, IAmazonSQS sqsClient, IConfiguration configuration)
        {
            _sqsClient = sqsClient;
            _logger = logger;

            _queueUrl = configuration.GetValue<string>("SqsQueueUrl")
                        ?? throw new ArgumentNullException("SqsQueueUrl is not set in appsettings");
        }

        public async Task<bool> PublishEventAsync<T>(T message)
        {
            try
            {
                var jsonMessage = JsonSerializer.Serialize(message);

                var request = new SendMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MessageBody = jsonMessage
                };

                var response = await _sqsClient.SendMessageAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("Message sent successfully to SQS: {MessageId}", response.MessageId);
                    return true;
                }

                _logger.LogError("Failed to send message to SQS. HTTP Status: {StatusCode}", response.HttpStatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending message to SQS.");
                return false;
            }
        }
    }

}
