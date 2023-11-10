using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQMessageReader.Consumer.Event;
using RabbitMQMessageReader.Message;
using System;
using System.Collections.Generic;

namespace RabbitMQMessageReader.Consumer
{
    public class Consumer : IDisposable
    {
        private readonly IModel channel;
        private readonly Publisher.Publisher publisher;
        private readonly Queue<RabbitMQMessage> readMessages;
        private readonly IList<byte[]> messageBodies;
        private readonly ConsumerOptions options;

        private EventingBasicConsumer consumer;
        private bool stopConsuming = false;
        private int readCount = 0;

        public string ConsumerTag { get; private set; }
        public DateTime LastReadDate { get; private set; }

        public delegate void ConsumingFinishedHandler(object sender, MessageConsumingFinishedEventArgs e);
        public event ConsumingFinishedHandler ConsumingFinished;

        public Consumer(IModel channel, IModel publisherChannel, ConsumerOptions options)
        {
            this.channel = channel;
            this.options = options;

            publisher = new(publisherChannel);
            readMessages = new Queue<RabbitMQMessage>();
            messageBodies = new List<byte[]>();
        }

        public string Start()
        {
            channel.BasicQos(0, options.PrefetchCount, false);

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            ConsumerTag = channel.BasicConsume(options.QueueName, false, consumer);

            return ConsumerTag;
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            if (stopConsuming)
            {
                return;
            }

            LastReadDate = DateTime.Now;

            readMessages.Enqueue(new(e.Exchange, e.RoutingKey, e.DeliveryTag, e.Body));
            messageBodies.Add(e.Body.ToArray());

            if (readMessages.Count == options.PrefetchCount)
            {
                ulong lastSuccessfullPublishedDeliveryTag = RepublishMessages();

                if (lastSuccessfullPublishedDeliveryTag > 0)
                {
                    channel.BasicAck(lastSuccessfullPublishedDeliveryTag, true);
                }

                if (readMessages.Count > 0)
                {
                    while (readMessages.Count > 1)
                    {
                        readMessages.Dequeue();
                    }

                    channel.BasicNack(readMessages.Dequeue().DeliveryTag, true, true);
                }
            }

            readCount++;

            if (readCount == options.ReadAmount)
            {
                ConsumingFinished?.Invoke(this, new MessageConsumingFinishedEventArgs(readCount));
            }
        }

        private ulong RepublishMessages()
        {
            ulong lastSuccessfullDeliveryTag = 0;

            while (readMessages.Count > 0)
            {
                RabbitMQMessage message = readMessages.Peek();

                try
                {
                    publisher.PublishMessage(message.Exchange, message.RoutingKey, message.Body);

                    lastSuccessfullDeliveryTag = message.DeliveryTag;

                    readMessages.Dequeue();
                }
                catch
                {
                    return lastSuccessfullDeliveryTag;
                }
            }

            return lastSuccessfullDeliveryTag;
        }

        public IList<byte[]> GetResult()
        {
            return messageBodies;
        }

        public void Dispose()
        {
            stopConsuming = true;
            consumer.Received -= Consumer_Received;

            if (channel.IsOpen)
            {
                channel.BasicCancel(ConsumerTag);
                channel.Close();
            }

            publisher.Dispose();
        }

    }
}