using Amido.Stacks.Configuration;

namespace Amido.Stacks.Messaging.AWS.SNS;

public class AwsSnsConfiguration
{
    public Secret TopicArn { get; init; }
}