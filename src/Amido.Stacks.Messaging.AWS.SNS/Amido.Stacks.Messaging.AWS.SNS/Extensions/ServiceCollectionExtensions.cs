using Amazon.SimpleNotificationService;
using Amido.Stacks.Application.CQRS.ApplicationEvents;
using Amido.Stacks.Messaging.AWS.SNS.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amido.Stacks.Messaging.AWS.SNS.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the AWS SNS client for IEventConsumer and IApplicationEventPublisher
        /// </summary>
        public static IServiceCollection AddAwsSns(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSimpleNotificationService>();
            services.AddTransient<IApplicationEventPublisher, EventPublisher>();

            return services;
        }
    }
}