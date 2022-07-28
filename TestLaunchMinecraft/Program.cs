using DaanV2.UUID;
using SeaMinecraftLauncherCore.Core.Model.Authentication;
using SeaMinecraftLauncherCore.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestLaunchMinecraft
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine(@"
   _____ __  __ _      _____               
  / ____|  \/  | |    / ____|              
 | (___ | \  / | |   | |     ___  _ __ ___ 
  \___ \| |\/| | |   | |    / _ \| '__/ _ \
  ____) | |  | | |___| |___| (_) | | |  __/
 |_____/|_|__|_|______\_____\___/|_|  \___|

  _____  ______ __  __  ____
 |  __ \|  ____|  \/  |/ __ \              
 | |  | | |__  | \  / | |  | |             
 | |  | |  __| | |\/| | |  | |             
 | |__| | |____| |  | | |__| |             
 |_____/|______|_|  |_|\____/ 
");
            try
            {
                var javaInfos = JavaTools.FindJava();
                Console.Write("请输入 .minecraft 路径：");
                string minecraftPath = Console.ReadLine();
                var versions = GameTools.FindVersion(minecraftPath);
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
                Console.Write("请输入测试功能（1：启动游戏 2：获取缺失 Assets 3：获取缺失 Libraries）：");
                string testFunction = Console.ReadLine();
                if (testFunction == "1")
                {
                    var java = JavaTools.AutoSelectJava(verInfo, javaInfos);
                    Console.WriteLine("\nJava 信息：");
                    for (int i = 1; i < javaInfos.Length + 1; i++)
                    {
                        Console.WriteLine($@"{i}：
版本：{javaInfos[i - 1].JavaVersion}
路径：{javaInfos[i - 1].JavaPath}");
                    }
                    Console.WriteLine($@"
已自动选择
版本：{java.JavaVersion}
路径：{java.JavaPath} 的 Java");
                    Console.Write("\n请输入分配的最大内存（单位：兆）：");
                    int memory = int.Parse(Console.ReadLine());
                    Console.Write("请输入验证模式（1: 微软登录 2: 离线登录）：");
                    int loginMode = int.Parse(Console.ReadLine());
                    string username;
                    string accessToken;
                    string uuid;
                    if (loginMode == 1)
                    {
                        Console.WriteLine("微软登录：开始");
                        Console.WriteLine("微软登录：正在进行 OAuth 验证");
                        string code = MicrosoftAuthenticator.MicrosoftOAuthAuthenticateAsync().Result;
                        Console.WriteLine("微软登录：正在进行 Token 验证");
                        var token = MicrosoftAuthenticator.MicrosoftTokenAuthenticateAsync(code).Result;
                        Console.WriteLine("微软登录：正在进行 XBL 验证");
                        var xblToken = MicrosoftAuthenticator.XBLAuthenticateAsync(token.Access_Token).Result;
                        Console.WriteLine("微软登录：正在进行 XSTS 验证");
                        var xstsToken = MicrosoftAuthenticator.XSTSAuthenticateAsync(xblToken.Token).Result;
                        Console.WriteLine("微软登录：正在获取 Minecraft Access Token");
                        accessToken = MicrosoftAuthenticator.MinecraftAuthenticateAsync(xstsToken).Result.AccessToken;
                        Console.WriteLine("微软登录：正在获取 Profile");
                        var profile = MicrosoftAuthenticator.GetProfileAsync(accessToken).Result;
                        username = profile.Username;
                        uuid = profile.UUID;
                    }
                    else
                    {
                        Console.Write("请输入用户名（离线登录）：");
                        var userInfo = OfflineAuthenticator.GetUserInfo(Console.ReadLine());
                        username = userInfo.Username;
                        accessToken = userInfo.AccessToken;
                        uuid = userInfo.UUID;
                    }
                    var gameArguments = new SeaMinecraftLauncherCore.Core.GameArguments
                    {
                        Username = username,
                        MaxMemory = memory,
                        UUID = uuid,
                        AccessToken = accessToken
                    };
                    string script = SeaMinecraftLauncherCore.Core.LaunchMinecraft.GenerateStartScript(verInfo, gameArguments, java);
                    Console.WriteLine(script);
                    Clipboard.SetText(script);
                    Console.WriteLine("\n已将此命令复制到剪贴板。");
                    Console.Write("请问是否要启动（Y）：");
                    string input = Console.ReadLine();
                    if (input.Equals("Y", StringComparison.OrdinalIgnoreCase))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            UseShellExecute = false,
                            FileName = "cmd",
                            RedirectStandardInput = true
                        };
                        Process process = new Process
                        {
                            StartInfo = startInfo
                        };
                        process.Start();
                        process.StandardInput.WriteLine("chcp 65001");
                        process.StandardInput.WriteLine(script);
                        process.StandardInput.WriteLine("exit");
                        process.WaitForExit();
                        process.Close();
                    }
                }
                else if (testFunction == "2")
                {
                    var assets = GameTools.GetMissingAssets(verInfo);
                    Console.WriteLine("以下是缺失 Assets 信息：");
                    foreach (var asset in assets.Assets)
                    {
                        Console.WriteLine($@"{asset.Key}：
SHA1：{asset.Value.SHA1}
大小（字节）：{asset.Value.Size}" + '\n');
                    }
                }
                else if (testFunction == "3")
                {
                    var libraries = GameTools.GetMissingLibraries(verInfo);
                    Console.WriteLine("以下是缺失 Libraries 信息：");
                    foreach (var library in libraries)
                    {
                        Console.WriteLine($@"{library.Name}：
路径：{library.Download?.Artifact.Path}
SHA1：{library.Download?.Artifact.Size}
大小（字节）：{library.Download?.Artifact.Size}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"出现错误，错误信息：
{ex}");
            }
            Console.WriteLine("\n按下回车键退出...");
            Console.ReadLine();
        }
    }
}
