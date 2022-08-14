using SeaMinecraftLauncherCore.Core.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Tools
{
    internal static class FileHelper
    {
        internal static FileInfo[] SearchFile(string searchPath, string fileName)
        {
            List<FileInfo> files = new List<FileInfo>();
            try
            {
                foreach (var file in Directory.GetFiles(searchPath))
                {
                    if (Path.GetFileName(file) == fileName)
                    {
                        files.Add(new FileInfo(file));
                    }
                }
            }
            catch { }
            try
            {
                foreach (var directory in Directory.GetDirectories(searchPath))
                {
                    try
                    {
                        files.AddRange(SearchFile(directory, fileName));
                    }
                    catch { }
                }
            }
            catch { }

            return files.ToArray();
        }

        internal static async Task<string> GetLocalVersion(string sha1)
        {
            VersionManifest versionManifest = await GameHelper.GetWebVersionInfo();
            foreach (var ver in versionManifest.Versions)
            {
                string verStr = await WebRequests.GetStringAsync(ver.Url);
                var verInfo = GameHelper.GetVanillaVersionInfoWithJsonString(verStr);
                if (verInfo.Downloads.Client.SHA1.Equals(sha1, StringComparison.OrdinalIgnoreCase))
                {
                    return verInfo.ID;
                }
            }
            throw new VersionNotFoundException($"未找到 SHA1 值为 {sha1} 的版本。");
        }
    }
}
