public interface IOssService
{
    void OssUpLoad(EventHandler<StreamTransferProgressArgs>? eventHandler = null);
}

public record OssOptions
{
    public string ossEndpoint { get; set; } = string.Empty;
    public string ossAccessKeyId { get; set; } = string.Empty;
    public string ossAccessKeySecret { get; set; } = string.Empty;
    public string ossBucketName { get; set; } = string.Empty;
    public string fileFullName { get; set; } = string.Empty;
    public string? reNamedFile { get; set; } = null;
}