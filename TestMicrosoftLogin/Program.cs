using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMicrosoftLogin
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("微软登录：开始");
            Console.WriteLine("微软登录：正在进行 OAuth 验证");
            string code = SeaMinecraftLauncherCore.Core.Model.Authentication.MicrosoftAuthenticator.MicrosoftOAuthAuthenticateAsync().Result;
            Console.WriteLine("微软登录：正在进行 Token 验证");
            var token = SeaMinecraftLauncherCore.Core.Model.Authentication.MicrosoftAuthenticator.MicrosoftTokenAuthenticateAsync(code).Result;
            Console.WriteLine("微软登录：正在进行 XBL 验证");
            var xblToken = SeaMinecraftLauncherCore.Core.Model.Authentication.MicrosoftAuthenticator.XBLAuthenticateAsync(token.Access_Token).Result;
            Console.WriteLine("微软登录：正在进行 XSTS 验证");
            var xstsToken = SeaMinecraftLauncherCore.Core.Model.Authentication.MicrosoftAuthenticator.XSTSAuthenticateAsync(xblToken.Token).Result;
            Console.WriteLine("微软登录：正在获取 Minecraft Access Token");
            string accessToken = SeaMinecraftLauncherCore.Core.Model.Authentication.MicrosoftAuthenticator.MinecraftAuthenticateAsync(xstsToken).Result.AccessToken;
            Console.WriteLine("微软登录：正在获取 Profile");
            var profile = SeaMinecraftLauncherCore.Core.Model.Authentication.MicrosoftAuthenticator.GetProfileAsync(accessToken).Result;
            Console.WriteLine($@"Username: {profile.Username}

AccessToken: {accessToken}

UUID： {profile.UUID}");
            Console.ReadLine();
        }
    }
}
