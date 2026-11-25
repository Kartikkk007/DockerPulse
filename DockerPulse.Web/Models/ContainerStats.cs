namespace DockerPulse.Web.Models;

public class ContainerStats
{
    public string ContainerId { get; set; } = "";

    public string ContainerName { get; set; } = "";

    public double CpuUsagePercent { get; set; }

    public double MemoryUsageMB { get; set; }

    public double MemoryLimitMB { get; set; }

    public double MemoryUsagePercent { get; set; }

    public DateTime LastUpdated { get; set; }
}