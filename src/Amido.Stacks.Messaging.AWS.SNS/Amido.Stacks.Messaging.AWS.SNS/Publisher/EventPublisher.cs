using System.Text.Json;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amido.Stacks.Application.CQRS.ApplicationEvents;
using Amido.Stacks.Configuration;
using Amido.Stacks.Messaging.AWS.SNS.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amido.Stacks.Messaging.AWS.SNS.Publisher
{
    /// <summary>
    /// Class implementing the ability to publish an event to AWS SNS
    /// </summary>
    public class EventPublisher : IApplicationEventPublisher
    {
        private readonly IOptions<AwsSnsConfiguration> configuration;
        private readonly ISecretResolver<string> secretResolver;
        private readonly IAmazonSimpleNotificationService snsClient;
        private readonly ILogger<EventPublisher> logger;

        public EventPublisher(
            IOptions<AwsSnsConfiguration> configuration,
            ISecretResolver<string> secretResolver,
            IAmazonSimpleNotificationService snsClient,
            ILogger<EventPublisher> logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.secretResolver = secretResolver ?? throw new ArgumentNullException(nameof(secretResolver));
            this.snsClient = snsClient ?? throw new ArgumentNullException(nameof(snsClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Publishes an event message to the configured SNS
        /// </summary>
        /// <param name="applicationEvent">The message object</param>
        /// <returns>Task</returns>
        public async Task PublishAsync(IApplicationEvent applicationEvent)
        {
            logger.PublishEventRequested(applicationEvent.CorrelationId.ToString());

            var topicArn = await secretResolver.ResolveSecretAsync(configuration.Value.TopicArn);
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var message = JsonSerializer.Serialize<object>(applicationEvent, jsonOptions);
            var messageRequest = new PublishRequest
            {
                Message = message,
                TopicArn = topicArn
            };

            try
            {
                await snsClient.PublishAsync(messageRequest);

                logger.PublishEventCompleted(applicationEvent.CorrelationId.ToString());
            }
            catch (AmazonSimpleNotificationServiceException exception)
            {
                logger.PublishEventFailed(applicationEvent.CorrelationId.ToString(), exception.Message, exception);
            }
            catch (AmazonClientException exception)
            {
                logger.PublishEventFailed(applicationEvent.CorrelationId.ToString(), exception.Message, exception);
            }
        }
    }
}