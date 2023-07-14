using RabbitMQSubcriber.Common.Events;

namespace RabbitMQSubcriber.Common.Exceptions
{
    public class OutboundMessageFailedException
         : Exception
    {
        public RabbitMessageOutbound RabbitMessageOutbound { get; set; }

        public OutboundMessageFailedException(string message, RabbitMessageOutbound outboundMessage) : base(message)
        {
            RabbitMessageOutbound = outboundMessage;
        }

        public OutboundMessageFailedException(string message, RabbitMessageOutbound outboundMessage, Exception inner) : base(message, inner)
        {
            RabbitMessageOutbound = outboundMessage;
        }
    }
}
