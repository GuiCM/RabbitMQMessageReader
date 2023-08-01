using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace RabbitMQMessageReader
{
    public class Consumer
    {
        private readonly IModel channel;

        private readonly IModel publisherChannel;
        
        private readonly Publisher publisher;

        private IList<ReadOnlyMemory<byte>> messageBodies;

        public Consumer(IModel channel, IModel publisherChannel)
        {
            this.channel = channel;
            this.publisherChannel = publisherChannel;

            publisher = new Publisher();
            messageBodies = new List<ReadOnlyMemory<byte>>(100);
        }

        public string Start()
        {            
            CustomPublisher customPublisher = new CustomPublisher();
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                //count++;

                // string body = Encoding.UTF8.GetString(e.Body.Span);               
                BasicGetResult mGet = channel.BasicGet("test-queue", false);

                messageBodies.Add(e.Body);

                //if (count == 2)
                {
                    customPublisher.Publish(publisherChannel, publisher, messageBodies, e.DeliveryTag);

                    messageBodies.Clear();

                    channel.BasicAck(e.DeliveryTag, true);
                }
            };

            return channel.BasicConsume("test-queue", false, consumer);
        }
    }
}