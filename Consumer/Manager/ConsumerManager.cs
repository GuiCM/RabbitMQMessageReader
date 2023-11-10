using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQMessageReader.Consumer.Event;

namespace RabbitMQMessageReader.Consumer.Manager
{
    public abstract class ConsumerManager : IConsumerManager
    {
        protected readonly IList<Consumer> consumers;
        // protected readonly IList<Consumer> finishedConsumers;

        public delegate void ConsumersFinishedHandler(object sender, ConsumersFinishedEventArgs e);
        public event ConsumersFinishedHandler ConsumersFinished;

        protected ConsumerManager()
        {
            consumers = new List<Consumer>();
            // finishedConsumers = new List<Consumer>();
        }

        public void AddConsumer(Consumer consumer)
        {
            consumers.Add(consumer);
        }

        public virtual void StartConsumers()
        {
            foreach (Consumer consumer in consumers)
            {
                consumer.ConsumingFinished += Consumer_ConsumingFinished;
                consumer.Start();
            }
        }

        protected virtual void Consumer_ConsumingFinished(object sender, MessageConsumingFinishedEventArgs e)
        {
            Consumer consumer = (Consumer)sender;

            lock (consumers) // One access to the list per time, to avoid multiple calls to the notify consumers finished
            {
                consumers.Remove(consumer);
                // finishedConsumers.Add(consumer);

                Task.Run(() => consumer.Dispose());

                NotifyConsumersFinished();
            }
        }

        protected virtual void NotifyConsumersFinished()
        {
            if (consumers.Count == 0)
            {
                InvokeConsumersFinished(this, new ConsumersFinishedEventArgs());
            }
        }

        protected void InvokeConsumersFinished(object sender, ConsumersFinishedEventArgs e)
        {
            ConsumersFinished?.Invoke(sender, e);
        }

    }
}