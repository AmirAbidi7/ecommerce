using main.Events;

namespace main.Service;

public interface IProducerService
{
    Task ProduceAsync(string topic, PurchaseEvent purchase);
}
