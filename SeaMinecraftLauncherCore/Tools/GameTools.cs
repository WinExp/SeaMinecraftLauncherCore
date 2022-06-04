using DaanV2.UUID;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeaMinecraftLauncherCore.Core.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Tools
{
    public static class GameTools
    {
        public static string GenerateOfflineUUID(string username)
        {
            return UUIDFactory.CreateUUID(3, 1, $"OfflinePlayer:{username}").ToString().Replace("-", "");
        }

        public static VanillaVersionInfo[] FindVersion(string versionPath)
        {
            List<VanillaVersionInfo> versions = new List<VanillaVersionInfo>();
            DirectoryInfo directoryInfo = new DirectoryInfo(versionPath);
            foreach (var directory in directoryInfo.GetDirectories())
            {
                var files = directory.GetFiles(directory.Name + ".json");
                versions.Add(GetVanillaVersionInfo(files[0].FullName));
            }

            return versions.ToArray();
        }

        public static VanillaVersionInfo GetVanillaVersionInfo(string jsonPath)
        {
            if (Path.GetExtension(jsonPath) != ".json")
            {
                throw new ArgumentException("Json 文件后缀错误。");
            }
            using (StreamReader reader = new StreamReader(jsonPath))
            {
                string jsonStr = reader.ReadToEnd();
                var verInfo = JsonConvert.DeserializeObject<VanillaVersionInfo>(jsonStr);
                verInfo.VersionPath = Path.GetDirectoryName(jsonPath);
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
                }
                return verInfo;
            }
        }
    }
}
