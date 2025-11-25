namespace Devkit.Common.Messaging.Options
{
    public class MessageBusOptions
    {
        public string? Provider { get; set; }
        public string? Host { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        public RabbitMqOptions? RabbitMQ { get; set; }
        public KafkaOptions? Kafka { get; set; }
        public RetryOptions? DefaultRetry { get; set; }
    }

    public class RabbitMqOptions
    {
        public int Port { get; set; } = 5672;
        public string? VirtualHost { get; set; } = "/";
    }

    public class KafkaOptions
    {
        public string? SecurityProtocol { get; set; }
        public Dictionary<string, string>? AdditionalConfig { get; set; }
    }

    public class RetryOptions
    {
        public int RetryLimit { get; set; } = 3;
        public int InitialIntervalSeconds { get; set; } = 5;
    }
}