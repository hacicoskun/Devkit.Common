namespace Devkit.Common.Logging.Options;

public class LoggingOptions
{
    public string ApplicationName { get; set; }
    public SeqOptions Seq { get; set; }
    public ElasticOptions Elastic { get; set; }
}

public class SeqOptions
{
    public bool IsEnabled { get; set; }
    public string ServerUrl { get; set; }
    public string ApiKey { get; set; }
}

public class ElasticOptions
{
    public bool IsEnabled { get; set; }
    public string NodeUrl { get; set; }
    public string IndexFormat { get; set; }
}