namespace RabbitMQLib.Constants
{
    public static class QueueNameConstant
    {
        public static readonly string DefaultDeadLetterExchangeName = "DeadLetterExchange";
        public static readonly string DefaultDeadLetterQueueName = "DeadLetterQueue";
        public static readonly string DefaultDeadLetterRoutingKey = "MessageQueue";
        public static readonly string DefaultExchangeName = "MessagesExchange";
        public static readonly string DefaultQueueName = "MessageQueue";
    }
}