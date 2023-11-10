using System;

namespace RabbitMQMessageReader.Consumer
{
    public class ConsumerOptions
    {
        private ushort prefetchCount;

        public int ReadAmount { get; set; }
        public string QueueName { get; set; }
        public ushort PrefetchCount
        {
            get
            {
                return prefetchCount;
            }
            set
            {
                prefetchCount = Math.Min(value, (ushort)100);
            }
        }

    }
}