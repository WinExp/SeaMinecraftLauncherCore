using DaanV2.UUID;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConsoleApp1
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
                var javaInfos = SeaMinecraftLauncherCore.Tools.JavaTools.FindJava();
                Console.Write("请输入 .minecraft 路径：");
                string minecraftPath = Console.ReadLine();
                var versions = SeaMinecraftLauncherCore.Tools.GameTools.FindVersion(Path.Combine(minecraftPath, "versions"));
                Console.WriteLine("版本信息：");
                for (int i = 1; i < versions.Length + 1; i++)
                {
                    Console.WriteLine($@"{i}：
名称：{versions[i - 1].ID}
版本：{versions[i - 1].ClientVersion}
路径：{versions[i - 1].VersionPath}");
                }
                Console.Write("\n请选择版本序号：");
                var verInfo = versions[int.Parse(Console.ReadLine()) - 1];
                var java = SeaMinecraftLauncherCore.Tools.JavaTools.AutoSelectJava(verInfo, javaInfos);
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
                Console.Write("请输入用户名（离线登录）：");
                string username = Console.ReadLine();
                string uuid = SeaMinecraftLauncherCore.Tools.GameTools.GenerateOfflineUUID(username);
                var gameArguments = new SeaMinecraftLauncherCore.Core.GameArguments
                {
                    Username = username,
                    MaxMemory = 1024,
                    UUID = uuid,
                    AccessToken = uuid
                };
                string script = SeaMinecraftLauncherCore.Core.LaunchMinecraft.GenerateStartScript(verInfo, gameArguments, java);
                Console.WriteLine(script);
                Clipboard.SetDataObject(script);
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
                    process.StandardInput.WriteLine("@echo off");
                    process.StandardInput.WriteLine(script);
                    process.StandardInput.WriteLine("exit");
                    process.WaitForExit();
                    process.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"出现错误，错误信息：
{ex.Message}");
            }
            Console.WriteLine("\n按下任意键退出...");
            Console.ReadKey();
        }
    }
}
