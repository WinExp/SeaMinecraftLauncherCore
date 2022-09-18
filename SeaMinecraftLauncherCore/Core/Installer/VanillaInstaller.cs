using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Installer
{
    public class VanillaInstaller
    {
        private string _path;

        public VanillaInstaller(string minecraftPath)
        {
            _path = minecraftPath;
        }
        
        public InstallProgress InstallVersion(Json.WebVersionInfo webVersion, string installName)
        {
            InstallProgress result = new InstallProgress();
            Task.Run(async () =>
            {
                try
                {
                    string verPath = Path.Combine(_path, "versions", installName);
                    if (!await DownloadCore.TryDownloadFileAsync(new DownloadCore.DownloadInfo(webVersion.Url, verPath)))
                    {
                        result.IsSuccess = false;
                        return;
                    }
                    Json.VanillaVersionInfo verInfo = Tools.GameHelper.GetVanillaVersionInfo(Path.Combine(verPath, installName + ".json"));
                    result.Progress = InstallProgress.ProgressEnum.Downloading_Client;
                    if (!await DownloadCore.TryDownloadFileAsync(new DownloadCore.DownloadInfo(verInfo.Downloads.Client.URL.Replace("https://launchermeta.mojang.com", "https://download.mcbbs.net").Replace("https://launcher.mojang.com", "https://download.mcbbs.net"), verPath), 30000))
                    {
                        result.IsSuccess = false;
                        return;
                    }
                    if (File.Exists(Path.Combine(verPath, installName + ".jar")))
                    {
                        File.Delete(Path.Combine(verPath, installName + ".jar"));
                    }
                    File.Move(Path.Combine(verPath, "client.jar"), Path.Combine(verPath, installName + ".jar"));
                    result.Progress = InstallProgress.ProgressEnum.Completing_Libraries;
                    var libraries = CompleteLibraries(verInfo);
                    libraries.Wait();
                    result.Progress = InstallProgress.ProgressEnum.Completing_Assets;
                    var assets = CompleteAssets(verInfo);
                    assets.Wait();
                    result.Progress = InstallProgress.ProgressEnum.Completing_Natives;
                    var natives = CompleteNatives(verInfo);
                    if (natives != null)
                    {
                        natives.Wait();
                    }
                    result.IsSuccess = true;
                }
                catch
                {
                    result.IsSuccess = false;
                }
                result.IsCompleted = true;
            });
            return result;
        }

        public static InstallProgress.CompleteProgress CompleteAssets(Json.VanillaVersionInfo verInfo)
        {
            string minecraftPath = Tools.PathExtension.GetMinecraftRootPath(verInfo.VersionPath);
            var assets = Tools.GameHelper.GetMissingAssets(verInfo, true);
            var downInfos = Tools.GameHelper.GetAssetsDownloadInfos(minecraftPath, assets);
            var downProgress = DownloadCore.TryDownloadFiles(downInfos, 2, 20000);
            return new InstallProgress.CompleteProgress(downInfos.Length, downProgress);
        }

        public static InstallProgress.CompleteProgress CompleteLibraries(Json.VanillaVersionInfo verInfo)
        {
            string minecraftPath = Tools.PathExtension.GetMinecraftRootPath(verInfo.VersionPath);
            var libraries = Tools.GameHelper.GetMissingLibraries(verInfo, true);
            var downInfos = Tools.GameHelper.GetLibrariesDownloadInfos(minecraftPath, libraries);
            var downProgress = DownloadCore.TryDownloadFiles(downInfos);
            return new InstallProgress.CompleteProgress(downInfos.Length, downProgress);
        }

        public static InstallProgress.CompleteProgress? CompleteNatives(Json.VanillaVersionInfo verInfo)
        {
            var natives = Tools.GameHelper.GetNatives(verInfo);
            if (natives.Length == 0)
            {
                return null;
            }
            string path = Path.Combine(Path.GetTempPath(), "SMLCore");
            var downInfos = new List<DownloadCore.DownloadInfo>();
            foreach (var native in natives)
            {
                downInfos.Add(new DownloadCore.DownloadInfo(native.URL.Replace("https://libraries.minecraft.net", "https://download.mcbbs.net/maven"), path));
            }
            var downProgress = DownloadCore.TryDownloadFiles(downInfos, 2, 15000);
            var result = new DownloadCore.DownloadProgress();
            Task.Run(() =>
            {
                while (downProgress.CompletedCount < downInfos.Count)
                {
                    foreach (var downInfo in downInfos)
                    {
                        string fileName = Path.Combine(downInfo.DownloadPath, Tools.PathExtension.GetUrlFileName(downInfo.Url));
                        if (File.Exists(fileName))
                        {
                            if (Tools.ZipHelper.UnzipFile(fileName, Path.Combine(verInfo.VersionPath, verInfo.ID + "-natives")) != 0)
                            {
                                result.FailedCount++;
                            }
                            result.CompletedCount++;
                        }
                    }
                }
                Directory.Delete(path, true);
            });
            return new InstallProgress.CompleteProgress(downInfos.Count, result);
        }
    }

    public class InstallProgress
    {
        public CompleteProgress LibrariesCompleteProgress { get; internal set; }
        public CompleteProgress AssetsCompleteProgress { get; internal set; }
        public CompleteProgress NativesCompleteProgress { get; internal set; }
        public bool IsCompleted { get; internal set; } = false;
        public bool IsSuccess { get; internal set; } = false;
        public ProgressEnum Progress { get; internal set; } = ProgressEnum.Downloading_Json;

        public class CompleteProgress
        {
            public int Length { get; private set; }
            public DownloadCore.DownloadProgress DownloadProgress { get; private set; }

            internal CompleteProgress(int length, DownloadCore.DownloadProgress progress)
            {
                Length = length;
                DownloadProgress = progress;
            }

            public void Wait()
            {
                while (DownloadProgress.CompletedCount < Length) { }
            }
        }

        public InstallProgress() { }

        public enum ProgressEnum
        {
            Downloading_Json,
            Downloading_Client,
            Completing_Libraries,
            Completing_Assets,
            Completing_Natives
        }
    }
}
