using App.DistributedEventBus;
using App.DistributedEventBus.Abstractions;
using App.EventBusServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Consumer
{
    class Program
    {
        private static IServiceProvider serviceProvider;
        private static string subscriptionClientName = "consumer-client";
        private static string serviceBusConnectionString = "service-bus-connection";

        static void Main(string[] args)
        {
            SetupContainer();
            Console.WriteLine("Hello World!");

            Console.WriteLine("Listening ...");
            Console.ReadLine();
        }

        private static void SetupContainer()
        {
            var serviceCollection = new ServiceCollection();

            ConfigDistributedEventBus(serviceCollection);
            serviceCollection.AddLogging(configure => configure.AddConsole());

            serviceProvider = serviceCollection.BuildServiceProvider();
            ConfigureEventBusSubscribers(serviceProvider);
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

            services.AddSingleton<IIntegrationEventHandler<TestIntegrationEvent>, TestIntegrationEventHandler>();

        }

        private static void ConfigureEventBusSubscribers(IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetService<IDistributedEventBus>();
            eventBus.Subscribe<TestIntegrationEvent, IIntegrationEventHandler<TestIntegrationEvent>>();   
        }
    }
}
