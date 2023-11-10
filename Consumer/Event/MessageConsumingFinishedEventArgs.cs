namespace RabbitMQMessageReader.Consumer.Event
{
    public class MessageConsumingFinishedEventArgs
    {
        public MessageConsumingFinishedEventArgs(int count)
        {
            Count = count;
        }

        public int Count { get; }
    }

}