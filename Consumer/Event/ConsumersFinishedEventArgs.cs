using System.Collections.Generic;

namespace RabbitMQMessageReader.Consumer.Event
{
    public class ConsumersFinishedEventArgs
    {
        public IList<byte[]> Result { get; set; }
    }

}