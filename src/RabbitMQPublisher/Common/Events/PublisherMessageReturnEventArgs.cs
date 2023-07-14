namespace RabbitMQPublisher.Common.Events
{
    /// <summary>
    /// Define event model when sending mesage is occurring error
    /// </summary>
    public class PublisherMessageReturnEventArgs : EventArgs
    {
        public RabbitMessageOutbound Message { get; set; }
        public string Reason { get; set; }

        public PublisherMessageReturnEventArgs(RabbitMessageOutbound Message, string Reason)
        {
            this.Message = Message;
            this.Reason = Reason;
        }
    }
}
