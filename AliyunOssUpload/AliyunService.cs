using Microsoft.Extensions.Options;

public class AliyunOssService : IOssService
{
    public AliyunOssService(IOptions<OssOptions> options)
    {
        this.Options = options;
    }

    public IOptions<OssOptions> Options { get; }

    public void OssUpLoad(EventHandler<StreamTransferProgressArgs>? eventHandler = null)
    {
        if (string.IsNullOrWhiteSpace(Options.Value.ossEndpoint))
        {
            throw new ArgumentException($"“{nameof(Options.Value.ossEndpoint)}”不能为 null 或空白。", nameof(Options.Value.ossEndpoint));
        }

        if (string.IsNullOrWhiteSpace(Options.Value.ossAccessKeyId))
        {
            throw new ArgumentException($"“{nameof(Options.Value.ossAccessKeyId)}”不能为 null 或空白。", nameof(Options.Value.ossAccessKeyId));
        }

        if (string.IsNullOrWhiteSpace(Options.Value.ossAccessKeySecret))
        {
            throw new ArgumentException($"“{nameof(Options.Value.ossAccessKeySecret)}”不能为 null 或空白。", nameof(Options.Value.ossAccessKeySecret));
        }

        if (string.IsNullOrWhiteSpace(Options.Value.ossBucketName))
        {
            throw new ArgumentException($"“{nameof(Options.Value.ossBucketName)}”不能为 null 或空白。", nameof(Options.Value.ossBucketName));
        }

        if (string.IsNullOrWhiteSpace(Options.Value.fileFullName))
        {
            throw new ArgumentException($"“{nameof(Options.Value.fileFullName)}”不能为 null 或空白。", nameof(Options.Value.fileFullName));
        }
        // 初始化OssClient
        var client = new OssClient(Options.Value.ossEndpoint, Options.Value.ossAccessKeyId, Options.Value.ossAccessKeySecret);

        string key = (string.IsNullOrWhiteSpace(Options.Value.reNamedFile) ? new FileInfo(Options.Value.fileFullName).Name : Options.Value.reNamedFile?.Trim()) + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".rar";
        string md5;
        using (var fs = File.Open(Options.Value.fileFullName, FileMode.Open))
        {
            md5 = OssUtils.ComputeContentMd5(fs, fs.Length);
            var objectMeta = new ObjectMetadata
            {
                ContentMd5 = md5
            };

            #region 1.校验上传方式，校验本地和上传后的文件MD5值，如果不一至则报异常。
            //client.PutObject(bucketName, key, fs, objectMeta);
            #endregion
            #region 2.简单上传方式
            //client.PutObject(bucketName, key, fileToUpload);
            #endregion
            #region 3.md5校验加上传进度显示方式
            var putObjectRequest = new PutObjectRequest(Options.Value.ossBucketName, key, fs, objectMeta);
            if (eventHandler != null)
                putObjectRequest.StreamTransferProgress += eventHandler;
            client.PutObject(putObjectRequest);
            #endregion
        }
        Console.WriteLine("Put object succeeded");
    }
}

