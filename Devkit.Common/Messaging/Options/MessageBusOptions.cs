namespace Devkit.Common.Messaging.Options;

public class MessageBusOptions
{
    public string? Provider { get; set; }   // RabbitMQ, Kafka, etc.
    public string? Host { get; set; }
    public int? Port { get; set; }
    public string? VirtualHost { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }

    // Exchange / Topic / Queue gibi kavramlar için genel alan
    public string? ChannelName { get; set; }

    // Routing veya mesaj tipi (direct, fanout, topic vb.)
    public string? RoutingType { get; set; }

    // QoS, priority gibi durumlar
    public int? PriorityLevel { get; set; }

    // Provider’a özel key-value ayarları
    public Dictionary<string, string>? AdditionalProperties { get; set; }
}