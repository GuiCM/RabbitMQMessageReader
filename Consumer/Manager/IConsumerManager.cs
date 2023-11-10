using static RabbitMQMessageReader.Consumer.Manager.ConsumerManager;

namespace RabbitMQMessageReader.Consumer.Manager
{
    public interface IConsumerManager
    {
        public event ConsumersFinishedHandler ConsumersFinished;

        void AddConsumer(Consumer consumer);

        void StartConsumers();

    }
}