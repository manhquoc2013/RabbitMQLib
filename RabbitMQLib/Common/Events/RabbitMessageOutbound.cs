namespace RabbitMQSubcriber.Common.Events
{
    public class RabbitMessageOutbound
    {
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string CorrelationId { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Expiration { get; internal set; } = string.Empty;

        public override string ToString()
        {
            return Message;
        }
    }
}
