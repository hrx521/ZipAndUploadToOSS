using Microsoft.Extensions.Options;

namespace AliyunOssUpload
{
    public interface IZipService
    {
        /// <summary>/// 利用压缩工作执行文件夹压缩     
        /// </summary>/// <param name="destinationFilePath">将要被压缩的文件夹（绝对路径）</param>
        /// <param name="zipedFileDri">压缩后的 .rar 的存放目录（绝对路径）</param>
        /// <param name="zipedFileName">压缩文件的名称（包括后缀）</param>
        /// <param name="sizePerFragment">分割压缩的分片大小，MB，0为不分割</param>
        /// <param name="zipPassword">解压缩密码：为压缩文件设置一个解压缩密码</param>
        /// <returns>true 或 false。压缩成功返回 true，反之，false。</returns> 
        bool DoZip();
    }
    public record ZipOptions
    {
        public string destinationFilePath { get; set; } = string.Empty;
        public string zipedFileDri { get; set; } = string.Empty;
        public string zipedFileName { get; set; } = string.Empty;
        public int sizePerFragment { get; set; } = 0;
        public string? zipPassword { get; set; } = null;
    }
}