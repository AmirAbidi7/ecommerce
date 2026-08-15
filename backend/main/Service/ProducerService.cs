using System.Text.Json;
using Confluent.Kafka;
using main.Events;

namespace main.Service;

public class ProducerService : IProducerService
{
    private readonly IConfiguration _configuration;
    private readonly IProducer<Null, string> _producer;

    public ProducerService(IConfiguration configuration)
    {
        _configuration = configuration;
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
        };
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    public Task ProduceAsync(string topic, PurchaseEvent purchase) =>
        ProduceAsync(topic, JsonSerializer.Serialize(purchase));

    public Task ProduceAsync(string topic, PromotionEvent promotion) =>
        ProduceAsync(topic, JsonSerializer.Serialize(promotion));

    private async Task ProduceAsync(string topic, string json)
    {
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
    }
}
