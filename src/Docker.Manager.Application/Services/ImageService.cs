namespace Docker.Manager.Application.Services;

/// <inheritdoc cref="IImageService"/>
public class ImageService : IImageService
{
    private readonly IDockerClient _dockerClient;

    public ImageService(
        IDockerClient dockerClient)
    {
        _dockerClient = dockerClient;
    }

    public Task<ImageDto> InspectImageAsync(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<ImageDto>> ListAsync(CancellationToken cancellationToken)
    {
        _dockerClient.Images.ListImagesAsync(new ImagesListParameters(), cancellationToken);
        throw new NotImplementedException();
    }
}
