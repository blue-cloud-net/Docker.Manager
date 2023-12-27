namespace Docker.Manager.Application.Mappers;

public class ImageInspectDto : ImageDto
{
    public string Id { get; set; } = String.Empty;

    public string Name { get; set; } = String.Empty;

    public DateTime Created { get; set; }
}
