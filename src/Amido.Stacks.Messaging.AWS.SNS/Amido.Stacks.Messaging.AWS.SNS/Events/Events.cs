namespace Amido.Stacks.Messaging.AWS.SNS.Events;

public enum EventCode
{
    GeneralException = 123456899,

    PublishEventRequested = 123456801,
    PublishEventCompleted = 123456802,
    PublishEventFailed = 123456803,
}