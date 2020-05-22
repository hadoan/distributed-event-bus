using App.DistributedEventBus.Abstractions;
using System;
using System.Threading.Tasks;

namespace Consumer
{
    public class TestIntegrationEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public bool Handled { get; private set; }

        public TestIntegrationEventHandler()
        {
            Handled = false;
        }

        public async Task Handle(TestIntegrationEvent @event)
        {
            Handled = true;
            Console.WriteLine($"Received TestIntegrationEvent at {@event.CreationDate}");
        }
    }
}
