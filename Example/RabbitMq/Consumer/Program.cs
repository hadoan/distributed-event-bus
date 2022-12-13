﻿using App.DistributedEventBus;
using App.DistributedEventBus.Abstractions;
using App.EventBusRabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace Consumer
{
    class Program
    {
        private static IServiceProvider serviceProvider;

        
        private static string rabbitMqUserName = "admin";
        private static string rabbitMqPwd = "rabbitMqPwd";
        private static int rabbitMqRetryCount = 5;
        private static string queueName = "test_queue";

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
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                ConnectionFactory factory = new ConnectionFactory();
                factory.Uri = new Uri("amqps://host:port");
                factory.UserName = rabbitMqUserName;
                factory.Password = rabbitMqPwd;
                factory.DispatchConsumersAsync = true;

                return new DefaultRabbitMQPersistentConnection(factory, logger, rabbitMqRetryCount);
            });
            services.AddSingleton<IDistributedEventBus, App.EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();

                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, services, eventBusSubcriptionsManager, queueName, rabbitMqRetryCount);
            });
            
            services.AddSingleton<IIntegrationEventHandler<TestIntegrationEvent>, TestIntegrationEventHandler>();

        }

        private static void ConfigureEventBusSubscribers(IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetService<IDistributedEventBus>();
            eventBus.Subscribe<TestIntegrationEvent, IIntegrationEventHandler<TestIntegrationEvent>>();   
        }
    }
}
