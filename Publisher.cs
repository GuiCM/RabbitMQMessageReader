using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQMessageReader
{
    public class Publisher
    {

        public void PublishMessage(IModel channel, object messageBody)
        {
            ReadOnlyMemory<byte> body = new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(messageBody));

            channel.BasicPublish("test-exchange", "test-rk", channel.CreateBasicProperties(), body);
        }

        public void PublishMessage(IModel channel, ReadOnlyMemory<byte> messageBody)
        {
            // ReadOnlyMemory<byte> body = new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(messageBody));

            channel.BasicPublish("test-exchange", "test-rk", channel.CreateBasicProperties(), messageBody);
        }
    }
}