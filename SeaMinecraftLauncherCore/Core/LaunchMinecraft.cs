using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeaMinecraftLauncherCore.Core.Json;
using SeaMinecraftLauncherCore.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core
{
    public static class LaunchMinecraft
    {
        public static string GenerateStartScript(VanillaVersionInfo versionInfo)
        {
            var jvmScript = new StringBuilder();

            foreach (string jvmArg in versionInfo.Arguments.JvmArgs)
            {
                jvmScript.Append(jvmArg);
                jvmScript.Append(' ');
            }
            StringBuilder classpath = new StringBuilder("\"");
            foreach (var library in versionInfo.Libraries)
            {
                if (library.Rules != null)
                {
                    foreach (var rule in library.Rules)
                    {
                        if (rule.Action == "allow")
                        {
                            if (rule.OS != null)
                            {
                                if (rule.OS.Name == "windows")
                                {
                                    continue;
                                }
                                else
                                {
                                    goto SkipLibrary;
                                }
                            }
                        }
                        else
                        {
                            if (rule.OS != null)
                            {
                                if (rule.OS.Name != "windows")
                                {
                                    continue;
                                }
                                else
                                {
                                    goto SkipLibrary;
                                }
                            }
                        }
                    }
                }
                classpath.Append(Path.Combine(PathExtension.GetMinecraftRootPath(versionInfo.VersionPath), library.Download.Artifact.Path.Replace('/', '\\')));
                classpath.Append(';');
            SkipLibrary:
                continue;
            }
            classpath.Append(Path.Combine(versionInfo.VersionPath, versionInfo.ID + ".jar"));
            classpath.Append('\"');
            jvmScript.Replace("${classpath}", classpath.ToString());
            jvmScript.Append(versionInfo.MainClass);
            jvmScript.Append(' ');


            var gameScript = new StringBuilder();
            if (versionInfo.OldMinecraftArguments != null)
            {
                gameScript.Append(versionInfo.OldMinecraftArguments);
                gameScript.Append(' ');
            }
            else
            {
                foreach (string gameArg in versionInfo.Arguments.GameArgs)
                {
                    gameScript.Append(gameArg);
                    gameScript.Append(' ');
                }
            }

            return jvmScript.ToString() + gameScript.ToString().Trim(' ');
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

    [Obsolete("临时使用，若此版本是正式版请提交 Issues。")]
    public class GameArguments
    {
        public GCMode GC;
        public int MinMemory;
        public int MaxMemory;
        public string Username;
        public int Width = 854;
        public int Height = 480;
        public string VersionType;
        public readonly string UUID = "000000000000000000000000000000";
        public readonly string AccessToken = "000000000000000000000000000000";

        public enum GCMode
        {
            G1GC,
            ZGC
        }
    }
}
