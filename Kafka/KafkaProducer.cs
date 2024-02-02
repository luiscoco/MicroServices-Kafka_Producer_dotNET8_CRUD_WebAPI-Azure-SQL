using Confluent.Kafka;

namespace AzureSQLWebAPIMicroservice.Kafka
{
    public class KafkaProducer
    {
        private readonly ProducerConfig _config;
        private readonly string _topic;

        public KafkaProducer(string bootstrapServers, string topic)
        {
            _config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _topic = topic;
        }

        public async Task SendMessageAsync(string key, string message)
        {
            using (var producer = new ProducerBuilder<string, string>(_config).Build())
            {
                try
                {
                    var result = await producer.ProduceAsync(_topic, new Message<string, string> { Key = key, Value = message });
                    Console.WriteLine($"Message sent to partition {result.Partition} with offset {result.Offset}");
                }
                catch (ProduceException<string, string> e)
                {
                    Console.WriteLine($"Error producing message: {e.Error.Reason}");
                }
            }
        }
    }
}
