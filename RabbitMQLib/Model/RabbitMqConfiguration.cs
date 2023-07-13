namespace RabbitMQLib.Model
{
    public class RabbitMqConfiguration
    {
        public string HostName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string VirtualHost { get; set; } = "/";
        public int Port { get; set; } = 5672;
        public TimeSpan SocketReadTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan SocketWriteTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
