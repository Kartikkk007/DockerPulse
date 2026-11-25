using Docker.DotNet;
using Docker.DotNet.Models;
using DockerPulse.Web.Models;
using System.Threading;

namespace DockerPulse.Web.Services;

public class DockerService
{
    private readonly DockerClient _dockerClient;

    public DockerService()
    {
        var configuration = new DockerClientConfiguration(
            new Uri("npipe://./pipe/docker_engine")
        );

        _dockerClient = configuration.CreateClient();
    }
    public async Task<ContainerStats> GetContainerStatsAsync(string containerId)
    {
        var statsCompletion = new TaskCompletionSource<ContainerStatsResponse>();

        var progress = new Progress<ContainerStatsResponse>(stats =>
        {
            statsCompletion.TrySetResult(stats);
        });

        await _dockerClient.Containers.GetContainerStatsAsync(
     containerId,
     new ContainerStatsParameters
     {
         Stream = false
     },
     progress,
     CancellationToken.None
 );

        var stats = await statsCompletion.Task;
        // Calculate CPU usage
        var cpuDelta =
            stats.CPUStats.CPUUsage.TotalUsage -
            stats.PreCPUStats.CPUUsage.TotalUsage;

        var systemCpuDelta =
            stats.CPUStats.SystemUsage -
            stats.PreCPUStats.SystemUsage;

        var numberOfCpus = stats.CPUStats.OnlineCPUs;

        if (numberOfCpus == 0)
        {
            numberOfCpus = (uint)(
                stats.CPUStats.CPUUsage.PercpuUsage?.Count ?? 1
            );
        }

        double cpuUsagePercent = 0;

        if (systemCpuDelta > 0 &&
            stats.CPUStats.CPUUsage.TotalUsage >=
            stats.PreCPUStats.CPUUsage.TotalUsage &&
            stats.CPUStats.SystemUsage >=
            stats.PreCPUStats.SystemUsage)
        {
            cpuUsagePercent =
                (double)cpuDelta / systemCpuDelta *
                numberOfCpus * 100.0;
        }

        return new ContainerStats
        {
            ContainerId = containerId,
            CpuUsagePercent = cpuUsagePercent,

            MemoryUsageMB = stats.MemoryStats.Usage / (1024.0 * 1024.0),

            MemoryLimitMB = stats.MemoryStats.Limit / (1024.0 * 1024.0),

            MemoryUsagePercent = stats.MemoryStats.Limit > 0
                ? stats.MemoryStats.Usage * 100.0 / stats.MemoryStats.Limit
                : 0,

            LastUpdated = DateTime.Now
        };
    }

    // GET ALL CONTAINERS
    public async Task<List<DockerContainer>> GetContainersAsync()
    {
        var containers = await _dockerClient.Containers.ListContainersAsync(
            new ContainersListParameters
            {
                All = true
            }
        );

        return containers.Select(container => new DockerContainer
        {
            Id = container.ID,

            Name = container.Names.FirstOrDefault()?.TrimStart('/') ?? "Unknown",

            Image = container.Image,

            State = container.State,

            Status = container.Status,

            Ports = string.Join(", ", container.Ports.Select(port =>
            port.PublicPort != 0
            ? $"{port.PublicPort}:{port.PrivatePort}"
            :  $"{port.PrivatePort}/{port.Type}"

            ))

        }).ToList();
    }

    // START CONTAINER
    public async Task StartContainerAsync(string containerId)
    {
        await _dockerClient.Containers.StartContainerAsync(
            containerId,
            new ContainerStartParameters()
        );
    }

    // STOP CONTAINER
    public async Task StopContainerAsync(string containerId)
    {
        await _dockerClient.Containers.StopContainerAsync(
            containerId,
            new ContainerStopParameters()
        );
    }

    // RESTART CONTAINER
    public async Task RestartContainerAsync(string containerId)
    {
        await _dockerClient.Containers.RestartContainerAsync(
            containerId,
            new ContainerRestartParameters()
        );
    }

}
