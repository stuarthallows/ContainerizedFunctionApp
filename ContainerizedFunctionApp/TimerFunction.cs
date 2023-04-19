using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues?tabs=passwordless#get-the-connection-string

namespace ContainerizedFunctionApp;

public class TimerFunction
{
    public TimerFunction()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        Config = configBuilder.Build();
    }

    private IConfigurationRoot Config { get; }

    /// <summary>
    /// Send a message to Azure Service Bus Queue on an interval.
    /// </summary>
    [FunctionName("TimerFunction")]
    public async Task Run([TimerTrigger("*/5 * * * * *")] TimerInfo timerInfo, ILogger log)
    {
        // set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
        // If you use the default AmqpTcp, you will need to make sure that the ports 5671 and 5672 are open
        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };

        var client = new ServiceBusClient(Config["ServiceBusConnection"], clientOptions);

        ServiceBusSender sender = client.CreateSender("test-queue");

        try
        {
            await sender.SendMessageAsync(new ServiceBusMessage(Guid.NewGuid().ToString()));
            log.LogInformation($"Published message to the queue");
        }
        finally
        {
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}
