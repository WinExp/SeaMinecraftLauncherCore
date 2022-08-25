using DaanV2.UUID;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeaMinecraftLauncherCore.Core;
using SeaMinecraftLauncherCore.Core.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Tools
{
    public static class GameHelper
    {
        /// <summary>
        /// 获取服务器上的 Version_Manifest 信息。
        /// </summary>
        /// <returns>Version_Manifest 信息。</returns>
        public static async Task<VersionManifest> GetWebVersionInfo()
        {
            string jsonStr = await WebRequests.GetStringAsync("https://launchermeta.mojang.com/mc/game/version_manifest.json");
            return JsonConvert.DeserializeObject<VersionManifest>(jsonStr);
        }

        /// <summary>
        /// 生成离线启动 UUID。
        /// </summary>
        /// <param name="username"></param>
        /// <returns>字符串类型 UUID。</returns>
        public static string GenerateOfflineUUID(string username)
        {
            return UUIDFactory.CreateUUID(3, 1, $"OfflinePlayer:{username}").ToString().Replace("-", "");
        }

        /// <summary>
        /// 查找当前目录下的 Minecraft 版本。
        /// </summary>
        /// <param name="minecraftPath"></param>
        /// <returns>查找到的 Minecraft 版本。</returns>
        public static VanillaVersionInfo[] FindVersions(string minecraftPath)
        {
            if (string.IsNullOrWhiteSpace(minecraftPath))
            {
                return new VanillaVersionInfo[0];
            }

            List<VanillaVersionInfo> versions = new List<VanillaVersionInfo>();
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(minecraftPath, "versions"));

            try
            {
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    try
                    {
                        foreach (var file in directory.GetFiles())
                        {
                            if (file.Name == directory.Name + ".json")
                            {
                                try
                                {
                                    versions.Add(GetVanillaVersionInfo(file.FullName));
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch
            {
                return new VanillaVersionInfo[0];
            }

            return versions.ToArray();
        }

        public static VanillaVersionInfo GetVanillaVersionInfoWithJsonString(string jsonStr)
        {
            var verInfo = JsonConvert.DeserializeObject<VanillaVersionInfo>(jsonStr);
            JObject json = JObject.Parse(jsonStr);
            JToken arguments;
            if (json.TryGetValue("arguments", out arguments))
            {
                List<string> gameArgsList = new List<string>();
                foreach (var arg in arguments.SelectToken("game"))
                {
                    if (arg.Type == JTokenType.String)
                    {
                        gameArgsList.Add(arg.ToString());
                    }
                }
                verInfo.Arguments.GameArgs = gameArgsList.ToArray();

                List<string> jvmArgsList = new List<string>() {
                        "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump",
                        "-Dlog4j2.formatMsgNoLookups=true",
                        "-XX:-UseAdaptiveSizePolicy",
                        "-XX:-OmitStackTraceInFastThrow",
                        "-Dfml.ignoreInvalidMinecraftCertificates=True",
                        "-Dfml.ignorePatchDiscrepancies=True"
                    };
                string backCommand = string.Empty;
                int exportCount = 0;
                foreach (var arg in arguments.SelectToken("jvm"))
                {
                    if (arg.Type == JTokenType.String && !jvmArgsList.Contains(arg.ToString()))
                    {
                        if (arg.ToString() == "--add-exports")
                        {
                            exportCount++;
                        }
                        if (exportCount > 0 && !arg.ToString().Contains("-") && backCommand != "--add-exports")
                        {
                            jvmArgsList.Add("--add-exports");
                        }
                        jvmArgsList.Add(arg.ToString());
                        backCommand = arg.ToString();
                    }
                }
                verInfo.Arguments.JvmArgs = jvmArgsList.ToArray();
            }
            return verInfo;
        }

        /// <summary>
        /// 通过 Json 路径获取版本 Json 内的版本信息。
        /// </summary>
        /// <param name="verJsonPath"></param>
        /// <returns>版本信息。</returns>
        /// <exception cref="ArgumentException"></exception>
        public static VanillaVersionInfo GetVanillaVersionInfo(string verJsonPath)
        {
            if (Path.GetExtension(verJsonPath) != ".json")
            {
                throw new ArgumentException("Json 文件后缀错误。");
            }
            using (StreamReader reader = new StreamReader(verJsonPath))
            {
                string jsonStr = reader.ReadToEnd();
                var verInfo = GetVanillaVersionInfoWithJsonString(jsonStr);
                verInfo.VersionPath = Path.GetDirectoryName(verJsonPath);
                return verInfo;
            }
        }

        /// <summary>
        /// 获取版本需要的 Assets。
        /// </summary>
        /// <param name="versionInfo"></param>
        /// <returns>Assets 信息。</returns>
        /// <exception cref="ArgumentException"></exception>
        public static AssetsIndexInfo GetAssets(VanillaVersionInfo versionInfo)
        {
            string minecraftPath = PathExtension.GetMinecraftRootPath(versionInfo.VersionPath);
            string assetsJsonPath = Path.Combine(minecraftPath, $"assets\\indexes\\{versionInfo.Assets}.json");
            if (Path.GetExtension(assetsJsonPath) != ".json")
            {
                throw new ArgumentException("Json 文件后缀错误。");
            }
            if (!File.Exists(assetsJsonPath))
            {
                DownloadCore.TryDownloadFileAsync(new DownloadCore.DownloadInfo(versionInfo.AssetIndex.URL.Replace("https://launchermeta.mojang.com/", "https://bmclapi2.bangbang93.com/"), Path.Combine(minecraftPath, "assets\\indexes"))).Wait();
            }
            using (StreamReader reader = new StreamReader(assetsJsonPath))
            {
                string jsonStr = reader.ReadToEnd();
                var assetsIndex = JsonConvert.DeserializeObject<AssetsIndexInfo>(jsonStr);
                return assetsIndex;
            }
        }

        /// <summary>
        /// 获取版本需要的 Natives。
        /// </summary>
        /// <param name="verInfo"></param>
        /// <returns>Natives 信息。</returns>
        public static VanillaVersionInfo.LibrariesClass.LibraryDownload.Download[] GetNatives(VanillaVersionInfo verInfo)
        {
            List<VanillaVersionInfo.LibrariesClass.LibraryDownload.Download> result = new List<VanillaVersionInfo.LibrariesClass.LibraryDownload.Download>();
            foreach (var library in verInfo.Libraries)
            {
                if (library.Download.Classifiers?.Windows != null)
                {
                    result.Add(library.Download.Classifiers.Windows);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 获取缺失 Libraries 信息。
        /// </summary>
        /// <param name="versionInfo"></param>
        /// <param name="validHash"></param>
        /// <returns>Libraries 信息。</returns>
        public static VanillaVersionInfo.LibrariesClass[] GetMissingLibraries(VanillaVersionInfo versionInfo, bool validHash = false)
        {
            string librariesPath = Path.Combine(PathExtension.GetMinecraftRootPath(versionInfo.VersionPath), "libraries");
            if (!Directory.Exists(librariesPath))
            {
                return versionInfo.Libraries.ToArray();
            }
            var result = new List<VanillaVersionInfo.LibrariesClass>();
            foreach (var library in versionInfo.Libraries)
            {
                try
                {
                    if (library.Download?.Artifact != null)
                    {
                        string libraryPath = Path.Combine(librariesPath, library.Download.Artifact.Path.Replace('/', '\\'));
                        bool isFileExist = false;
                        bool isHashCorrect = false;
                        bool isRuleCorrect = false;
                        if (library.Rules != null)
                        {
                            foreach (var rule in library.Rules)
                            {
                                if (rule.Action == "allow")
                                {
                                    if (rule.OS == null || rule.OS.Name == "windows")
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        goto SkipRules;
                                    }
                                }
                                else
                                {
                                    if (rule.OS == null || rule.OS.Name != "windows")
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        goto SkipRules;
                                    }
                                }
                            }
                        }
                        isRuleCorrect = true;
                    SkipRules:
                        if (isRuleCorrect && File.Exists(libraryPath))
                        {
                            isFileExist = true;

                            if (validHash)
                            {
                                string hash = HashHelper.GetFileHash(libraryPath, "SHA1");
                                if (hash == library.Download.Artifact.SHA1)
                                {
                                    isHashCorrect = true;
                                }
                            }
                        }
                        if (isRuleCorrect && !isFileExist && (!validHash || !isHashCorrect))
                        {
                            result.Add(library);
                        }
                    }
                }
                catch { }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 获取缺失 Assets 信息。
        /// </summary>
        /// <param name="versionInfo"></param>
        /// <param name="validHash"></param>
        /// <returns>缺失 Assets 信息。</returns>
        public static AssetsIndexInfo GetMissingAssets(VanillaVersionInfo versionInfo, bool validHash = false)
        {
            string assetsPath = Path.Combine(PathExtension.GetMinecraftRootPath(versionInfo.VersionPath), "assets");
            var assets = GetAssets(versionInfo);
            if (!Directory.Exists(assetsPath))
            {
                return assets;
            }
            var resultAssets = new AssetsIndexInfo();
            foreach (var asset in assets.Assets)
            {
                try
                {
                    string assetPath = Path.Combine(assetsPath, $"objects\\{asset.Value.SHA1.Substring(0, 2)}\\{asset.Value.SHA1}");
                    bool isHashCorrect = false;
                    bool isFileExist = false;
                    if (File.Exists(assetPath))
                    {
                        isFileExist = true;
                        if (validHash)
                        {
                            string hash = HashHelper.GetFileHash(assetPath, "SHA1");
                            if (hash == asset.Value.SHA1)
                            {
                                isHashCorrect = true;
                            }
                        }
                    }
                    if (!resultAssets.Assets.ContainsValue(asset.Value) && !isFileExist && (!validHash || !isHashCorrect))
                    {
                        resultAssets.Assets.Add(asset.Key, asset.Value);
                    }
                }
                catch { }
            }
            return resultAssets;
        }

        /// <summary>
        /// 转换 Libraries 信息为 DownloadInfo 信息。
        /// </summary>
        /// <param name="libraries"></param>
        /// <returns>转换后的 DownloadInfo 信息。</returns>
        public static DownloadCore.DownloadInfo[] GetLibrariesDownloadInfos(string minecraftPath, VanillaVersionInfo.LibrariesClass[] libraries)
        {
            List<DownloadCore.DownloadInfo> downInfos = new List<DownloadCore.DownloadInfo>();
            foreach (var library in libraries)
            {
                if (library.Rules != null)
                {
                    foreach (var rule in library.Rules)
                    {
                        if ((rule.Action == "allow" && rule.OS == null) || (rule.Action == "allow" && rule.OS.Name == "windows") || (rule.Action == "disallow" && rule.OS.Name != "windows"))
                        {
                            continue;
                        }
                        goto SkipLibrary;
                    }
                }
                if (library.Download?.Artifact != null)
                {
                    var downInfo = new DownloadCore.DownloadInfo(library.Download.Artifact.URL.Replace("https://libraries.minecraft.net", "https://download.mcbbs.net/maven"), Path.Combine(minecraftPath, "libraries", Path.GetDirectoryName(library.Download.Artifact.Path.Replace('/', '\\'))), library.Download.Artifact.SHA1);
                    downInfos.Add(downInfo);
                }
            SkipLibrary:
                continue;
            }
            return downInfos.ToArray();
        }

        public static DownloadCore.DownloadInfo[] GetAssetsDownloadInfos(string minecraftPath, AssetsIndexInfo assets)
        {
            List<DownloadCore.DownloadInfo> downInfos = new List<DownloadCore.DownloadInfo>();
            List<long> downInfoSizes = new List<long>();
            foreach (var asset in assets.Assets.Values)
            {
                if (asset == null)
                {
                    continue;
                }
                //http://resources.download.minecraft.net/
                //https://bmclapi2.bangbang93.com/assets/
                //https://download.mcbbs.net/assets/
                var addDownInfo = new DownloadCore.DownloadInfo($"http://resources.download.minecraft.net/{asset.SHA1.Substring(0, 2)}/{asset.SHA1}",
                    Path.Combine(minecraftPath, "assets\\objects", asset.SHA1.Substring(0, 2)), asset.SHA1);
                if (downInfos.Count > 1)
                {
                    long size = downInfoSizes.Max();
                    if (asset.Size >= size)
                    {
                        int idx = downInfoSizes.IndexOf(size);
                        downInfos.Insert(idx, addDownInfo);
                        downInfoSizes.Insert(idx, asset.Size);
                        continue;
                    }
                }
                downInfos.Add(addDownInfo);
                downInfoSizes.Add(asset.Size);
            }
            return downInfos.ToArray();
        }
    }
}
