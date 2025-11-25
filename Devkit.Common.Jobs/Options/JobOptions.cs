namespace Devkit.Common.Jobs.Options;

public class JobOptions
{
    public string Provider { get; set; } = "Hangfire";
    public bool UseQueue { get; set; } = false;
    public JobStorageOptions Storage { get; set; } = new();
    public List<JobScheduleOptions> Schedules { get; set; } = new();
}

public class JobStorageOptions
{
    public string Type { get; set; } = "PostgreSQL"; 
    public string? ConnectionString { get; set; }
}

public class JobScheduleOptions
{
    public string JobName { get; set; } = default!;
    public string Cron { get; set; } = default!;
    public bool? UseQueue { get; set; }
}