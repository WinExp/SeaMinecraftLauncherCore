using DaanV2.UUID;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
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
                var javaInfo = SeaMinecraftLauncherCore.Tools.JavaTools.FindJava();

                Console.WriteLine("Java 信息：");
                for (int i = 1; i < javaInfo.Length + 1; i++)
                {
                    Console.WriteLine($@"{i}：
版本：{javaInfo[i - 1].JavaVersion}
路径 {javaInfo[i - 1].JavaPath}");
                }

                Console.Write("请选择 Java 编号：");
                var java = javaInfo[int.Parse(Console.ReadLine()) - 1];
                Console.Write("\n请输入分配的最大内存（单位：兆）：");
                int memory = int.Parse(Console.ReadLine());
                Console.Write("\n请输入版本路径（只测试了原版）：");
                string inputVerPath = Console.ReadLine();
                string verPath = Path.Combine(inputVerPath, Path.GetFileName(inputVerPath) + ".json");
                var verInfo = SeaMinecraftLauncherCore.Core.LaunchMinecraft.GetVanillaVersionInfo(verPath);
                Console.Write("请输入用户名（离线登录）：");
                string username = Console.ReadLine();
                string uuid = UUIDFactory.CreateUUID(3, 1, $"OfflinePlayer:{username}").ToString().Replace("-", "");
                var gameArguments = new SeaMinecraftLauncherCore.Core.GameArguments
                {
                    Username = username,
                    MaxMemory = 1024,
                    UUID = uuid,
                    AccessToken = uuid
                };
                string script = SeaMinecraftLauncherCore.Core.LaunchMinecraft.GenerateStartScript(verInfo, gameArguments, java);
                Console.WriteLine(script);
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
