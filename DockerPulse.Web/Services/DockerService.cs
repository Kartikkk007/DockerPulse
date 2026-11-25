using Docker.DotNet;
using Docker.DotNet.Models;
using DockerPulse.Web.Models;

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