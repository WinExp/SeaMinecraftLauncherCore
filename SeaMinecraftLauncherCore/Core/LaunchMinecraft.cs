using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeaMinecraftLauncherCore.Core.Json;
using SeaMinecraftLauncherCore.Core.Model.Authentication;
using SeaMinecraftLauncherCore.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SeaMinecraftLauncherCore.Core
{
    public static class LaunchMinecraft
    {
        /// <summary>
        /// 生成启动脚本。
        /// </summary>
        /// <param name="versionInfo"></param>
        /// <param name="gameArguments"></param>
        /// <param name="javaInfo"></param>
        /// <returns>启动脚本。</returns>
        public static string GenerateStartScript(VanillaVersionInfo versionInfo, GameArguments gameArguments, JavaInfo javaInfo)
        {
            var jvmScript = new StringBuilder($"\"{javaInfo.JavaPath}\" -XX:+Use{gameArguments.GC} ");
            var minecraftRootPath = PathExtension.GetMinecraftRootPath(versionInfo.VersionPath);
            string libraryPath = Path.Combine(minecraftRootPath, "libraries");

            foreach (string jvmArg in versionInfo.Arguments?.JvmArgs ?? versionInfo.OldVanillaJvmArgs)
            {
                jvmScript.Append(jvmArg.Replace(" ", ""));
                jvmScript.Append(' ');
            }
            jvmScript.Replace("${natives_directory}", '\"' + Path.Combine(versionInfo.VersionPath, versionInfo.ID + "-natives") + '\"');
            jvmScript.Replace("${library_directory}", '\"' + libraryPath + '\"');
            jvmScript.Replace("${classpath_separator}", ";");

            bool isNewForge = false;
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
                if (library.Download?.Artifact != null)
                {
                    if (library.Name.Contains("net.minecraftforge") && Version.Parse(versionInfo.Assets) > new Version(1, 13))
                    {
                        isNewForge = true;
                    }
                    string path = Path.Combine(libraryPath, library.Download.Artifact.Path.Replace('/', '\\'));
                    if (classpath.ToString().Contains(path))
                    {
                        goto SkipLibrary;
                    }
                    classpath.Append(path);
                    classpath.Append(';');
                }
                else
                {
                    string[] names = library.Name.Split(':');
                    names[0] = names[0].Replace('.', '\\');
                    classpath.Append(Path.Combine(libraryPath, string.Join("\\", names), names[names.Length - 2] + '-' + names[names.Length - 1] + ".jar"));
                    classpath.Append(';');
                }
            SkipLibrary:
                continue;
            }
            if (isNewForge)
            {
                classpath.Remove(classpath.Length - 1, 1);
            }
            else
            {
                classpath.Append(Path.Combine(versionInfo.VersionPath, versionInfo.ID + ".jar"));
            }
            classpath.Append('\"');
            jvmScript.Replace("${classpath}", classpath.ToString());
            jvmScript.Replace("${version_name}", '\"' + versionInfo.ID + '\"');
            jvmScript.Append($"-Xmn256m -Xmx{gameArguments.MaxMemory}m ");
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
            gameScript.Replace("${auth_player_name}", gameArguments.Username)
                .Replace("${version_name}", '\"' + versionInfo.ID + '\"')
                .Replace("${game_directory}", '\"' + versionInfo.VersionPath + '\"')
                .Replace("${assets_root}", '\"' + Path.Combine(minecraftRootPath, "assets") + '\"')
                .Replace("${assets_index_name}", versionInfo.AssetIndex.ID)
                .Replace("${auth_uuid}", gameArguments.UUID)
                .Replace("${auth_access_token}", gameArguments.AccessToken)
                .Replace("${user_type}", "Mojang")
                .Replace("${version_type}", gameArguments.VersionType);

            gameScript.Append($"--width {gameArguments.Width} --height {gameArguments.Height}");
            return jvmScript.ToString() + gameScript.ToString();
        }
    }

    public class GameArguments
    {
        public GCMode GC { get; set; } = GCMode.G1GC;

        public int MaxMemory { get; set; }

        public int Width { get; set; } = 854;

        public int Height { get; set; } = 480;

        public string VersionType { get; set; } = "Minecraft";

        public string Username { get; set; }

        public string AccessToken { get; set; }

        public string UUID { get; set; }

        public enum GCMode
        {
            G1GC,
            ZGC
        }

        public GameArguments() { }
    }
}
