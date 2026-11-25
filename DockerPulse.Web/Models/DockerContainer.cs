namespace DockerPulse.Web.Models;

public class DockerContainer
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    public string Image { get; set; } = "";

    public string State { get; set; } = "";

    public string Status { get; set; } = "";

    public string Ports { get; set; } = "";
}