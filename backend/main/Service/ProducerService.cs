using Confluent.Kafka;
using main.Events;

namespace main.Service;

public class ProducerService
{
    private readonly IConfiguration _configuration;

    private readonly IProducer<Null, PurchaseEvent> _EmailNotificiationProducer;

    public ProducerService(IConfiguration configuration)
    {
        _configuration = configuration;
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
        };

        _EmailNotificiationProducer = new ProducerBuilder<Null, PurchaseEvent>(
            producerConfig
        ).Build();
    }

    public async Task ProduceAsync(string topic, PurchaseEvent purchase)
    {
        var kafkaMessage = new Message<Null, PurchaseEvent> { Value = purchase };
        await _EmailNotificiationProducer.ProduceAsync(topic, kafkaMessage);
    }
}
