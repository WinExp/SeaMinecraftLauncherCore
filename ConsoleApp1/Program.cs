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
            var verInfo = SeaMinecraftLauncherCore.Core.LaunchMinecraft.GetVanillaVersionInfo(@"D:\Lzr\Minecraft\.minecraft\versions\1.18.2 Universal\1.18.2 Universal.json");
            string script = SeaMinecraftLauncherCore.Core.LaunchMinecraft.GenerateStartScript(verInfo);
            Console.WriteLine(script);
            Console.ReadKey();
        }
    }
}
