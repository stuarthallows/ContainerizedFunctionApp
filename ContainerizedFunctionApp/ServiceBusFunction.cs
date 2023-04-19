using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ContainerizedFunctionApp;

public class ServiceBusFunction
{
    [FunctionName("ServiceBusFunction")]
    public void Run([ServiceBusTrigger("test-queue", Connection = "ServiceBusConnection")] string myQueueItem, ILogger log)
    {
        log.LogInformation($"Read message from the queue: {myQueueItem}");
    }
}
