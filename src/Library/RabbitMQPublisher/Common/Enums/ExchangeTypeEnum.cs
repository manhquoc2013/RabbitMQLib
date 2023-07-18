namespace RabbitMQPublisher.Common.Enums
{
    /// <summary>
    /// Define type of exchange in RabbitMQ
    /// </summary>
    public enum ExchangeTypeEnum
    {
        direct = 0,
        fanout = 1,
        headers = 2,
        topic = 3
    }
}
