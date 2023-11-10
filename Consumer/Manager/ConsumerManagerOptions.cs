namespace RabbitMQMessageReader.Consumer.Manager
{
    public class ConsumerManagerOptions
    {
        public int messageCount { get; set; }
        public string queueName { get; set; }
        public ushort prefetchCount { get; set; }
        public ushort preferredConsumerCount { get; set; }
    }

}