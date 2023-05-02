using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ContainerizedFunctionApp;

public class ServiceBusFunction
{
    [FunctionName("ServiceBusFunction")]
    public void Run([ServiceBusTrigger("test-queue", Connection = "ServiceBusConnection")] string myQueueItem, ILogger log)
    {
        // The properties set on the scope do not make it into the log.
        using (log.BeginScope(new Dictionary<string, object> { { "FunctionName", GetType().Name } }))
        {
            log.LogInformation($"Read message from the queue: {myQueueItem}");
        }
    }
}
