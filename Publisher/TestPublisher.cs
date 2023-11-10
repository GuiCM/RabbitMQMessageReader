using RabbitMQ.Client;
using RabbitMQMessageReader.Message;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace RabbitMQMessageReader.Publisher
{
    public class TestPublisher
    {
        private readonly IModel channel;
        private readonly Publisher publisher;

        public TestPublisher([NotNull] IModel channel)
        {
            this.channel = channel;

            publisher = new Publisher(channel);
        }

        public void PublishTestMessage(int amount)
        {
            try
            {
                channel.ConfirmSelect();

                /*channel.BasicAcks += (sender, ea) =>
                {
                    ackInvoked++;
                    ackTest += ea.DeliveryTag + "\n";
                };

                channel.BasicNacks += (sender, ea) =>
                {
                    nackInvoked++;
                    nackTest += ea.DeliveryTag + "\n";
                };*/

                for (int i = 0; i < amount; i++)
                {
                    publisher.PublishMessage(ConfigurationManager.DeadLetterExchange, ConfigurationManager.MainRoutingKey, new TestMessage { MessageId = i });
                }

                channel.WaitForConfirms(TimeSpan.FromSeconds(3));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // MessageBox.Show("Ack Callback invoked count: " + ackInvoked + " - Messages acked: " + ackTest);
            // MessageBox.Show("Nack Callback invoked count: " + nackInvoked + " - Messages nacked: " + nackTest);
        }
    }
}
