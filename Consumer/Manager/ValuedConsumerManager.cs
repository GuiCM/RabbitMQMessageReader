using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQMessageReader.Consumer.Event;

namespace RabbitMQMessageReader.Consumer.Manager
{
    public class ValuedConsumerManager : ConsumerManager
    {
        private IList<byte[]> consumersResult;

        public ValuedConsumerManager()
            : base()
        {
            consumersResult = new List<byte[]>();
        }

        protected override void Consumer_ConsumingFinished(object sender, MessageConsumingFinishedEventArgs e)
        {
            Consumer consumer = (Consumer)sender;

            lock (consumers) // One access to the list per time, to avoid multiple calls to the notify consumers finished
            {
                consumersResult = consumersResult.Concat(consumer.GetResult()).ToList();
                consumers.Remove(consumer);
                // finishedConsumers.Add(consumer);

                Task.Run(() => consumer.Dispose());

                NotifyConsumersFinished();
            }
        }

        protected override void NotifyConsumersFinished()
        {
            if (consumers.Count == 0)
            {
                InvokeConsumersFinished(this, new ConsumersFinishedEventArgs() { Result = consumersResult });
            }
        }
    }
}