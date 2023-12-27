namespace Docker.Manager.Application.Mappers;

/// <summary>
/// 镜像
/// </summary>
public class ImageDto
{
    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; set; } = String.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = String.Empty;

    /// <summary>
    /// 构建时间
    /// </summary>
    public DateTime Created { get; set; }
}
