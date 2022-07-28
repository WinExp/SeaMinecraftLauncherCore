using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeaMinecraftLauncherCore.Core.Form;
using SeaMinecraftLauncherCore.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SeaMinecraftLauncherCore.Core.Model.Authentication
{
    public static class MicrosoftAuthenticator
    {
        /// <summary>
        /// OAuth 验证（在窗口中）。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception.LoginCancelException"></exception>
        public static async Task<string> MicrosoftOAuthAuthenticateAsync()
        {
            await Task.Delay(0);
            string authUrl = $"https://login.live.com/oauth20_authorize.srf?client_id=00000000402b5328&response_type=code&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL&prompt=login&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf";
            return await StartForm1.Start(authUrl) ?? throw new Exception.LoginCancelException("用户取消了登录。");
        }

        /// <summary>
        /// OAuth Token 验证。
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static async Task<Json.MicrosoftJson> MicrosoftTokenAuthenticateAsync(string code)
        {
            string authUrl = $"https://login.live.com/oauth20_token.srf";
            string authContent = $"client_id=00000000402b5328&code={code}&grant_type=authorization_code&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf";
            string jsonStr = await WebRequests.GetPostStringAsync(authUrl, authContent, "application/x-www-form-urlencoded");
            Json.MicrosoftJson json = JsonConvert.DeserializeObject<Json.MicrosoftJson>(jsonStr);
            return json;
        }

        /// <summary>
        /// OAuth RefreshToken 验证。
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public static async Task<Json.MicrosoftJson> MicrosoftRefreshTokenAuthenticateAsync(string refreshToken)
        {
            string authUrl = $"https://login.live.com/oauth20_token.srf";
            string authContent = $"client_id=00000000402b5328&refresh_token={refreshToken}&grant_type=authorization_code&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf";
            string jsonStr = await WebRequests.GetPostStringAsync(authUrl, authContent, "application/x-www-form-urlencoded");
            Json.MicrosoftJson json = JsonConvert.DeserializeObject<Json.MicrosoftJson>(jsonStr);
            return json;
        }

        /// <summary>
        /// XBL 验证。
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<Json.XboxToken> XBLAuthenticateAsync(string token)
        {
            string authUrl = "https://user.auth.xboxlive.com/user/authenticate";
            string authContent = "{\"Properties\":{\"AuthMethod\":\"RPS\",\"SiteName\":\"user.auth.xboxlive.com\",\"RpsTicket\":\"" + token + "\"},\"RelyingParty\":\"http://auth.xboxlive.com\",\"TokenType\":\"JWT\"}";
            string jsonStr = await WebRequests.GetPostStringAsync(authUrl, authContent);
            Json.XboxToken json = JsonConvert.DeserializeObject<Json.XboxToken>(jsonStr);
            return json;
        }

        /// <summary>
        /// XSTS 验证。
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<Json.XboxToken> XSTSAuthenticateAsync(string token)
        {
            string authUrl = "https://xsts.auth.xboxlive.com/xsts/authorize";
            string authContent = "{\"Properties\":{\"SandboxId\":\"RETAIL\",\"UserTokens\":[\"" + token + "\"]},\"RelyingParty\":\"rp://api.minecraftservices.com/\",\"TokenType\":\"JWT\"}";
            string jsonStr = await WebRequests.GetPostStringAsync(authUrl, authContent);
            Json.XboxToken json = JsonConvert.DeserializeObject<Json.XboxToken>(jsonStr);
            return json;
        }

        /// <summary>
        /// 获取 Minecraft AccessToken。
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<Json.MinecraftTokenJson> MinecraftAuthenticateAsync(Json.XboxToken token)
        {
            string authUrl = "https://api.minecraftservices.com/authentication/login_with_xbox";
            string authContent = "{\"identityToken\":\"XBL3.0 x=" + token.DisplayClaims.Xui[0].UserHash + ";" + token.Token + "\"}";
            string jsonStr = await WebRequests.GetPostStringAsync(authUrl, authContent);
            Json.MinecraftTokenJson json = JsonConvert.DeserializeObject<Json.MinecraftTokenJson>(jsonStr);
            return json;
        }

        /// <summary>
        /// 获取 Profile。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception.AccountNotExistException"></exception>
        public static async Task<Json.MinecraftProfileJson> GetProfileAsync(string accessToken)
        {
            string authUrl = "https://api.minecraftservices.com/minecraft/profile";
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", $"Bearer {accessToken}");
            string jsonStr = await WebRequests.GetStringAsync(authUrl, headers);
            JObject jsonObj = JObject.Parse(jsonStr);
            string error = jsonObj.Value<string>("error");
            if (error == "NOT_FOUND")
            {
                throw new Exception.AccountNotExistException("该账户中不存在 Minecraft 账户。");
            }
            Json.MinecraftProfileJson json = JsonConvert.DeserializeObject<Json.MinecraftProfileJson>(jsonStr);
            return json;
        }
    }
}
