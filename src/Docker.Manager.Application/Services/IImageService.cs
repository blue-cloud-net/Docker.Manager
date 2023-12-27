namespace Docker.Manager.Application.Services;

/// <summary>
/// 镜像服务
/// </summary>
public interface IImageService
{
    /// <summary>
    /// 列出镜像
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<ImageDto>> ListAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 描述镜像
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ImageDto> InspectImageAsync(string name, CancellationToken cancellationToken);
}
