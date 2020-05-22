using System;
using App.DistributedEventBus;
using App.DistributedEventBus.Abstractions;
using App.EventBusRabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Publisher
{
    class Program
    {
        private static string rabbitMqHostName = "localhost";
        private static string rabbitMqUserName = "guest";
        private static string rabbitMqPwd = "guest";
        private static int rabbitMqRetryCount = 5;
        private static string queueName = "test_queue";

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
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = rabbitMqHostName,
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(rabbitMqUserName))
                {
                    factory.UserName = rabbitMqUserName;
                }

                if (!string.IsNullOrEmpty(rabbitMqPwd))
                {
                    factory.Password = rabbitMqPwd;
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, rabbitMqRetryCount);
            });
            services.AddSingleton<IDistributedEventBus, App.EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();

                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, services, eventBusSubcriptionsManager, queueName, rabbitMqRetryCount);
            });
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }

    }
}
