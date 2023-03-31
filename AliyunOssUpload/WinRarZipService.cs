using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System.Diagnostics;

namespace AliyunOssUpload
{
    public class WinRarZipService : IZipService
    {
        public WinRarZipService(IOptions<ZipOptions> options)
        {
            ValidateZipOptions(options);
            Options = options;
        }

        /// <summary>
        /// WinRAR安装注册表key  
        /// </summary>
        private const string WinRAR_KEY = @"WinRAR.ZIP\shell\open\command";

        public IOptions<ZipOptions> Options { get; set; }

        public bool DoZip()
        {
            return DoZip(Options);
        }

        private void ValidateZipOptions(IOptions<ZipOptions> options)
        {

            if (string.IsNullOrWhiteSpace(options.Value.destinationFilePath))
            {
                throw new ArgumentException($"“{nameof(options.Value.destinationFilePath)}”不能为 null 或空白。", nameof(options.Value.destinationFilePath));
            }

            if (string.IsNullOrWhiteSpace(options.Value.zipedFileDri))
            {
                throw new ArgumentException($"“{nameof(options.Value.zipedFileDri)}”不能为 null 或空白。", nameof(options.Value.zipedFileDri));
            }

            if (string.IsNullOrWhiteSpace(options.Value.zipedFileName))
            {
                throw new ArgumentException($"“{nameof(options.Value.zipedFileName)}”不能为 null 或空白。", nameof(options.Value.zipedFileName));
            }
        }
        /// <summary>/// 利用 WinRAR 进行压缩     
        /// </summary>/// <param name="destinationFilePath">将要被压缩的文件夹（绝对路径）</param>
        /// <param name="zipedFileDri">压缩后的 .rar 的存放目录（绝对路径）</param>
        /// <param name="zipedFileName">压缩文件的名称（包括后缀）</param>
        /// <param name="sizePerFragment">分割压缩的最大大小，MB，0为不分割</param>
        /// <returns>true 或 false。压缩成功返回 true，反之，false。</returns> 
        /// 
        public bool DoZip(IOptions<ZipOptions> options)
        {

            ValidateZipOptions(options);

            bool isZipDoneAndExited = false;
            string winRarexeFilePath;       //WinRAR.exe 的完整路径      
            RegistryKey? registryKey;  //注册表键       
            object registryValue;     //键值       
            string cmd;          //WinRAR 命令参数      
            ProcessStartInfo startinfo;
            Process process;

            registryKey = Registry.ClassesRoot.OpenSubKey(WinRAR_KEY) ?? throw new KeyNotFoundException(WinRAR_KEY);
            
            registryValue = registryKey.GetValue("") ?? throw new KeyNotFoundException(nameof(registryValue)+ "registryKey.GetValue(\"\")");  // 键值为 "d:\Program Files\WinRAR\WinRAR.exe" "%1"          
            winRarexeFilePath = $"{registryValue}";
            registryKey.Close();
            winRarexeFilePath = winRarexeFilePath.Substring(1, winRarexeFilePath.Length - 7);  // d:\Program Files\WinRAR\WinRAR.exe
            Directory.CreateDirectory(options.Value.destinationFilePath);             //压缩命令，相当于在要压缩的文件夹(path)上点右键->WinRAR->添加到压缩文件->输入压缩文件名(rarName)
            if (string.IsNullOrWhiteSpace(options.Value.zipPassword))
            {
                cmd = string.Format("a {0} {1} -r -v{2}m {0} -o+", options.Value.zipedFileName, options.Value.destinationFilePath, options.Value.sizePerFragment);
            }
            else
                cmd = string.Format("a {0} {1} -r -v{2}m {0} -p{3} -o+", options.Value.zipedFileName, options.Value.destinationFilePath, options.Value.sizePerFragment, options.Value.zipPassword);

            startinfo = new ProcessStartInfo();
            startinfo.FileName = winRarexeFilePath;
            startinfo.Arguments = cmd;                          //设置命令参数      
            startinfo.WindowStyle = ProcessWindowStyle.Hidden;  //隐藏 WinRAR 窗口      
            startinfo.WorkingDirectory = options.Value.zipedFileDri;
            process = new Process();
            process.StartInfo = startinfo;
            process.Start();
            process.WaitForExit(); //无限期等待进程 winrar.exe 退出      
            if (process.HasExited)
            {
                isZipDoneAndExited = true;
            }
            process.Close();
            return isZipDoneAndExited;
        }
    }
}
