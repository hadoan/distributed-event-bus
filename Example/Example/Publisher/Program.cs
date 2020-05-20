using System;
using System.Threading;
using App.DistributedEventBus;
using App.DistributedEventBus.Abstractions;
using App.EventBusServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Publisher
{
    class Program
    {
        private static string subscriptionClientName = "publisher-client";
        private static string serviceBusConnectionString = "service-bus-connection";
        private static ServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            SetupContainer();

            var eventBus = serviceProvider.GetService<IDistributedEventBus>();

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"Sending test event ... {i}");
                eventBus.Publish(new TestIntegrationEvent());

                System.Threading.Tasks.Task.Delay(1000);
            }
            Console.WriteLine("Done, press enter to continue");
            Console.ReadLine();
        }


        private static void SetupContainer()
        {
            var serviceCollection = new ServiceCollection();
           
            ConfigDistributedEventBus(serviceCollection);
            serviceCollection.AddLogging(configure => configure.AddConsole());
            serviceProvider = serviceCollection.BuildServiceProvider();
        }


        private static void ConfigDistributedEventBus(IServiceCollection services)
        {  
            services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();
                var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);
                return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            });
            services.AddSingleton<IDistributedEventBus, App.EventBusServiceBus.EventBusServiceBus>(sp =>
            {
                var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusServiceBus(serviceBusPersisterConnection, logger,
                    eventBusSubcriptionsManager, subscriptionClientName, services);
            });
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }

    }
}
