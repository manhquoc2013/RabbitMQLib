namespace RabbitMQPublisher.Common.Exceptions
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException(string message) : base(message)
        {

        }

        public NotConnectedException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
