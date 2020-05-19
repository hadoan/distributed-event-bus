using System;
using App.DistributedEventBus.Abstractions;
using App.EventBusServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Publisher
{
    class Program
    {
        private static ServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            SetupContainer();


        }


        private static void SetupContainer()
        {
            serviceProvider = new ServiceCollection()
                    
                     .BuildServiceProvider();

        }


        private void ConfigDistributedEventBus(IServiceCollection services)
        {
            //var subscriptionClientName = "SubscriptionClientName";
            //services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            //{
            //    var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();

            //    var serviceBusConnectionString = "EventBusConnection";
            //    var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

            //    return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            //});
            //services.AddSingleton<IDistributedEventBus, App.EventBusServiceBus.EventBusServiceBus>(sp =>
            //{
            //    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
            //    var iLifetimeScope = sp.GetRequiredService<IScopedIocResolver>();
            //    var logger = sp.GetRequiredService<ILogger<EventBusServiceBus.EventBusServiceBus>>();
            //    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            //    return new EventBusServiceBus.EventBusServiceBus(serviceBusPersisterConnection, logger,
            //        eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
            //});
            //services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            //services.AddSingleton<IIntegrationEventHandler<JobUpdateByDriverIntegrationEvent>, JobUpdateByDriverEventHandler>();
            //services.AddSingleton<IIntegrationEventHandler<RecalculateDriverStatusIntegrationEvent>, RecalculateDriverStatusIntegrationEventHandler>();
            //services.AddSingleton<IIntegrationEventHandler<SendbirdChatMessageAddedIntegrationEvent>, SendbirdChatMessageAddedIntegrationEventHandler>();

        }

    }
}
