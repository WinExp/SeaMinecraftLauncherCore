using System;
using System.Threading.Tasks;

namespace TestDownload
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestDownload();
            Console.ReadKey();
        }

        static async Task TestDownload()
        {
            Console.Write("请输入 .minecraft 路径：");
            string minecraftPath = Console.ReadLine();
            var versions = SeaMinecraftLauncherCore.Tools.GameHelper.FindVersion(minecraftPath);
            Console.WriteLine("版本信息：");
            for (int i = 1; i < versions.Length + 1; i++)
            {
                Console.WriteLine($@"{i}：
名称：{versions[i - 1].ID}
版本：{versions[i - 1].Assets}
路径：{versions[i - 1].VersionPath}");
            }
            Console.Write("\n请选择版本序号：");
            var verInfo = versions[int.Parse(Console.ReadLine()) - 1];

            DateTime startTime = DateTime.Now;
            /*
            SeaMinecraftLauncherCore.Tools.DownloadCore.DownloadSuccess += (s, e) =>
            {
                completedCount++;
                if (e.Success)
                {
                    Console.WriteLine($"{e.DownloadInfo.Url} 下载成功，还剩 {downInfos.Length - completedCount} 个");
                }
                else
                {
                    failedCount++;
                    Console.WriteLine($"{e.DownloadInfo.Url} 下载失败，还剩 {downInfos.Length - completedCount} 个");
                }
            };
            */
            var progress = SeaMinecraftLauncherCore.Core.Installer.VanillaInstaller.CompleteAssets(verInfo);
            Console.WriteLine($"开始下载 {progress.Length} 个文件");
            //int completedCount = 0;
            while (progress.DownloadProgress.CompletedCount < progress.Length)
            {
                /*
                if (completedCount < progress.DownloadProgress.CompletedCount)
                {
                    completedCount = progress.DownloadProgress.CompletedCount;
                    Console.WriteLine($"下载完成，还剩 {progress.Length - progress.DownloadProgress.CompletedCount} 个，目前错误 {progress.DownloadProgress.FailedCount} 个");
                }
                */
                Console.WriteLine($"还剩 {progress.Length - progress.DownloadProgress.CompletedCount} 个，目前错误 {progress.DownloadProgress.FailedCount} 个");
                await Task.Delay(1000);
            }

            Console.WriteLine($"{progress.Length} 个文件下载完成，错误 {progress.DownloadProgress.FailedCount} 个，耗时 {(DateTime.Now - startTime).TotalSeconds} 秒");
            var missingAssets = SeaMinecraftLauncherCore.Tools.GameHelper.GetMissingAssets(verInfo, true);
            Console.WriteLine($"Assets 缺失 {missingAssets.Assets.Count} 个");
        }
    }
}
