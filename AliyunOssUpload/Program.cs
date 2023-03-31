using AliyunOssUpload;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private async static Task Main(string[] args)
    {
        ZipOptions zipOptions = new ZipOptions();
        OssOptions ossOptions = new OssOptions();

        var hostBuilder = Host.CreateDefaultBuilder(args);
        var host = hostBuilder.ConfigureAppConfiguration((hostBuilderContext, configuration) =>
        {
            configuration.Sources.Clear();
            IHostEnvironment env = hostBuilderContext.HostingEnvironment;

            IConfigurationRoot configurationRoot = configuration
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true).Build();
        })
        .ConfigureServices((hostBuilderContext, services) =>
        {
            //将配置注入到容器,第一种方式
            services.Configure<ZipOptions>(hostBuilderContext.Configuration.GetSection(nameof(ZipOptions)));
            services.Configure<OssOptions>(hostBuilderContext.Configuration.GetSection(nameof(OssOptions)));
            ////将配置注入到容器,第二种方式
            //services.AddOptions<ZipOptions>().Bind(hostBuilderContext.Configuration.GetSection(nameof(ZipOptions)));
            //services.AddOptions<OssOptions>().Bind(hostBuilderContext.Configuration.GetSection(nameof(OssOptions)));


            //将服务注入到容器
            services.AddTransient<IZipService, WinRarZipService>()
                .AddTransient<IOssService, AliyunOssService>();
        })
        .Build();

        try
        {
            if (args.Contains("z") || args.Contains("Z"))
            {
                Console.WriteLine();
                Console.WriteLine("开始压缩。。。");
                var zipService = host.Services.GetService<IZipService>() ?? throw new KeyNotFoundException("host.Services.GetService<IZipService>()");
                zipService.DoZip();
                Console.WriteLine("压缩完成，正准备上传文件。。。");
            }
            if (args.Contains("a") || args.Contains("A"))
            {
                await Task.Delay(1000);
                Console.WriteLine("开始上传文件。。。");
                var ossService = host.Services.GetService<IOssService>() ?? throw new KeyNotFoundException("host.Services.GetService<IOssService>()");
                Status.WriteProgressBar(0, false);
                EventHandler<StreamTransferProgressArgs> eventHandler = new EventHandler<StreamTransferProgressArgs>((sender,e)=> Status.WriteProgressBar(e.PercentDone, true));
                ossService.OssUpLoad(eventHandler);
                Console.WriteLine("上传成功！3秒后将自动退出");
                await Task.Delay(3000);
            }
        }
        catch (Exception ex) { Console.WriteLine(ex); Console.ReadKey(); }
    }
}