using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var versions = SeaMinecraftLauncherCore.Tools.GameTools.FindVersion(minecraftPath);
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

            //var downInfos = SeaMinecraftLauncherCore.Tools.GameTools.GetLibrariesDownloadInfos(minecraftPath, SeaMinecraftLauncherCore.Tools.GameTools.GetMissingLibraries(verInfo, true));
            var assets = SeaMinecraftLauncherCore.Tools.GameTools.GetMissingAssets(verInfo);
            var downInfos = SeaMinecraftLauncherCore.Tools.GameTools.GetAssetsDownloadInfos(minecraftPath, assets);
            DateTime startTime = DateTime.Now;
            int completedCount = 0;
            int failedCount = 0;
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
            Console.WriteLine($"开始下载 {downInfos.Length} 个文件");
            var asyncPool = await SeaMinecraftLauncherCore.Tools.DownloadCore.TryDownloadFiles(downInfos);
            while (asyncPool.Count != 0)
            {
                foreach (var task in asyncPool)
                {
                    if (task.IsCompleted)
                    {
                        completedCount++;
                        if (task.IsFaulted)
                        {
                            Console.WriteLine($"失败，还剩 {downInfos.Length - completedCount} 个");
                            failedCount++;
                        }
                        else
                        {
                            Console.WriteLine($"成功，还剩 {downInfos.Length - completedCount} 个");
                        }
                        asyncPool.Remove(task);
                        break;
                    }
                }
            }
            
            Console.WriteLine($"{downInfos.Length} 个文件下载完成，错误 {failedCount} 个，耗时 {(DateTime.Now - startTime).TotalSeconds} 秒");
            var missingLibraries = SeaMinecraftLauncherCore.Tools.GameTools.GetMissingLibraries(verInfo, true);
            Console.WriteLine($"Libraries 缺失 {missingLibraries.Length} 个");
        }
    }
}
