using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestInstallVersion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestInstallAsync().Wait();
            Console.ReadKey();
        }
        
        static async Task TestInstallAsync()
        {
            var minecraftList = await SeaMinecraftLauncherCore.Tools.GameHelper.GetWebVersionInfo();
            Console.Write("请输入需要安装的版本：");
            string installVersionStr = Console.ReadLine();
            Console.Write("请输入安装路径：");
            string minecraftPath = Console.ReadLine();
            var installer = new SeaMinecraftLauncherCore.Core.Installer.VanillaInstaller(minecraftPath);
            DateTime startTime = DateTime.Now;
            SeaMinecraftLauncherCore.Core.Installer.InstallProgress progress;
            try
            {
                progress = await installer.InstallVersion(installVersionStr, installVersionStr);
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("未找到版本");
                return;
            }
            SeaMinecraftLauncherCore.Core.Installer.InstallProgress.ProgressEnum? lastProgress = null;
            while (!progress.IsCompleted)
            {
                if (lastProgress != progress.Progress)
                {
                    lastProgress = progress.Progress;
                    switch (lastProgress)
                    {
                        case SeaMinecraftLauncherCore.Core.Installer.InstallProgress.ProgressEnum.Downloading_Json:
                            Console.WriteLine($"安装进度：正在下载版本 Json");
                            break;
                        case SeaMinecraftLauncherCore.Core.Installer.InstallProgress.ProgressEnum.Downloading_Client:
                            Console.WriteLine($"安装进度：正在下载版本核心");
                            break;
                        case SeaMinecraftLauncherCore.Core.Installer.InstallProgress.ProgressEnum.Completing_LibrariesAssetsNatives:
                            Console.WriteLine("安装进度：正在补全 Libraries, Assets, Natives");
                            break;

                    }
                }
            }
            if (progress.IsSuccess)
            {
                Console.WriteLine($"安装成功，耗时 {(DateTime.Now - startTime).TotalSeconds} 秒");
            }
            else
            {
                Console.WriteLine($"安装失败，耗时 {(DateTime.Now - startTime).TotalSeconds} 秒");
            }
        }
    }
}
