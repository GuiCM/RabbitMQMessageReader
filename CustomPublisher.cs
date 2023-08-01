using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;

namespace RabbitMQMessageReader
{
    public class CustomPublisher
    {
        private int ackInvoked;
        private string ackTest;

        private int nackInvoked;
        private string nackTest;

        public void Publish(IModel channel, Publisher publisher)
        {
            try
            {
                channel.ConfirmSelect();

                channel.BasicAcks += (sender, ea) =>
                {
                    ackInvoked++;
                    ackTest += ea.DeliveryTag + "\n";
                };

                channel.BasicNacks += (sender, ea) =>
                {
                    nackInvoked++;
                    nackTest += ea.DeliveryTag + "\n";
                };

                for (int i = 0; i < 1; i++)
                {
                    publisher.PublishMessage(channel, new TestMessage { MessageId = i });
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

        public void Publish(IModel channel, Publisher publisher, IList<ReadOnlyMemory<byte>> bodies, ulong deliveryTag)
        {
            try
            {
                channel.ConfirmSelect();

                channel.BasicAcks += (sender, ea) =>
                {
                    ackInvoked++;
                    ackTest += ea.DeliveryTag + " - ";
                };

                channel.BasicNacks += (sender, ea) =>
                {
                    nackInvoked++;
                    nackTest += ea.DeliveryTag + " - ";
                };

                for (int i = 0; i < bodies.Count; i++)
                {
                    publisher.PublishMessage(channel, bodies[i]);
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
