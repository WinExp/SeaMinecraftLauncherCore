using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeaMinecraftLauncherCore.Core.Json;
using SeaMinecraftLauncherCore.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SeaMinecraftLauncherCore.Core
{
    public static class LaunchMinecraft
    {
        public static string GenerateStartScript(VanillaVersionInfo versionInfo, GameArguments gameArguments, JavaInfo javaInfo)
        {
            var jvmScript = new StringBuilder($"\"{javaInfo.JavaPath}\" -XX:+Use{gameArguments.GC} ");
            var minecraftRootPath = PathExtension.GetMinecraftRootPath(versionInfo.VersionPath);

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
                classpath.Append(Path.Combine(minecraftRootPath, "libraries", library.Download.Artifact.Path.Replace('/', '\\')));
                classpath.Append(';');
            SkipLibrary:
                continue;
            }
            classpath.Append(Path.Combine(versionInfo.VersionPath, versionInfo.ID + ".jar"));
            classpath.Append('\"');
            jvmScript.Replace("${classpath}", classpath.ToString());
            jvmScript.Append(versionInfo.MainClass);
            jvmScript.Append($" -Xmn256m -Xmx{gameArguments.MaxMemory}m ");


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
            gameScript.Replace("${auth_player_name}", gameArguments.Username)
                .Replace("${version_name}", versionInfo.ID)
                .Replace("${game_directory}", versionInfo.VersionPath)
                .Replace("${assets_root}", Path.Combine(minecraftRootPath, "assets"))
                .Replace("${assets_index_name}", versionInfo.AssetIndex.ID)
                .Replace("${auth_uuid}", gameArguments.UUID)
                .Replace("${auth_access_token}", gameArguments.AccessToken)
                .Replace("${user_type}", "Mojang")
                .Replace("${version_type}", gameArguments.VersionType);

            return jvmScript.ToString() + gameScript.ToString().Trim(' ');
        }
    }

    [Obsolete("临时使用，若此版本是正式版请提交 Issues。")]
    public class GameArguments
    {
        public GCMode GC = GCMode.G1GC;
        public int MaxMemory;
        public string Username;
        public int Width = 854;
        public int Height = 480;
        public string VersionType = "Minecraft";
        public string UUID;
        public string AccessToken;

        public enum GCMode
        {
            G1GC,
            ZGC
        }

        public GameArguments() { }
    }
}
