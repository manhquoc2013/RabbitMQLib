namespace RabbitMQSubcriber.Common.Enums
{
    public enum OverflowBehaviorEnum
    {
        None = 0,
        DropHead = 1,
        RejectPublish = 2
    }
}
