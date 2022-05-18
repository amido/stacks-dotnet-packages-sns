using Amido.Stacks.Messaging.AWS.SNS.Events;
using Microsoft.Extensions.Logging;

namespace Amido.Stacks.Messaging.AWS.SNS.Logging
{
    /// <summary>
    /// Contains log definitions for SNS component
    /// LoggerMessage.Define() creates a unique template for each log type
    /// The log template reduces the number of allocations and write logs faster to destination
    /// </summary>
    public static class LogDefinition
    {
        /// Failures with exceptions should be logged to respective failures(i.e: getByIdFailed) and then to logException in order to show them as separate entries in the logs(trace + exception
        private static readonly Action<ILogger, string, Exception> logException =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId((int)EventCode.GeneralException, nameof(EventCode.GeneralException)),
                "AWS SNS Exception: {Message}"
            );

        // Publishing
        private static readonly Action<ILogger, string, Exception> publishEventRequested =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId((int)EventCode.PublishEventRequested, nameof(EventCode.PublishEventRequested)),
                "AWS SNS: PublishAsync requested for CorrelationId:{CorrelationId}"
            );

        private static readonly Action<ILogger, string, Exception> publishEventCompleted =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId((int)EventCode.PublishEventCompleted, nameof(EventCode.PublishEventCompleted)),
                "AWS SNS: PublishAsync completed for CorrelationId:{CorrelationId}"
            );

        private static readonly Action<ILogger, string, string, Exception> publishEventFailed =
            LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId((int)EventCode.PublishEventFailed, nameof(EventCode.PublishEventFailed)),
                "AWS SNS: PublishAsync failed for CorrelationId:{CorrelationId}. Reason:{Reason}"
            );

        //Exception

        /// <summary>
        /// When an exception is present in the failure, it will be logged as exception message instead of trace.
        /// Logging messages with an exception will make them an exception and the trace will lose an entry, making harder to debug issues
        /// </summary>
        private static void LogException(ILogger logger, Exception exception)
        {
            if (exception is not null)
                logException(logger, exception.Message, exception);
        }

        // Publishing
        public static void PublishEventRequested(this ILogger logger, string correlationId)
        {
            publishEventRequested(logger, correlationId, null!);
        }

        public static void PublishEventCompleted(this ILogger logger, string correlationId)
        {
            publishEventCompleted(logger, correlationId, null!);
        }

        public static void PublishEventFailed(this ILogger logger, string correlationId, string reason, Exception ex)
        {
            publishEventFailed(logger, correlationId, reason, null!);
            LogException(logger, ex);
        }
    }
}