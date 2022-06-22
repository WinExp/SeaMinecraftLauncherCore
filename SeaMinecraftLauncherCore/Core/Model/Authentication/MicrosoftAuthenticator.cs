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
    public sealed class MicrosoftAuthenticator
    {
        public string Username { get; set; }
        public string UUID { get; set; }
        public string AccessToken { get; set; }

        public async Task<string> MicrosoftOAuthAuthenticateAsync()
        {
            await Task.Delay(0);
            string authUrl = $"https://login.live.com/oauth20_authorize.srf?client_id=c479c08e-68bd-429c-9345-324d8b1e6010&response_type=code&redirect_uri={HttpUtility.UrlEncode("http://localhost")}&scope=XboxLive.signin%20offline_access";
            return StartForm1.Start(authUrl);
        }

        public async Task<Json.MicrosoftJson> MicrosoftTokenAuthenticateAsync(string code)
        {
            string authUrl = $"https://login.live.com/oauth20_token.srf";
            string authContent = $"client_id=c479c08e-68bd-429c-9345-324d8b1e6010&client_secret=GXq8Q~lcj5u_BbJSTLeT-ILB0HxEke~7PRSFXa45&code={code}&grant_type=authorization_code&redirect_uri={HttpUtility.UrlEncode("http://localhost")}";
            string jsonStr = await WebRequests.GetPostStringAsync(authUrl, authContent, "application/x-www-form-urlencoded");
            Json.MicrosoftJson json = JsonConvert.DeserializeObject<Json.MicrosoftJson>(jsonStr);
            return json;
        }

        public async Task<Json.XboxToken> XBLAuthenticateAsync(string token)
        {
            string authUrl = "https://user.auth.xboxlive.com/user/authenticate";
            string authContent = "{\"Properties\":{\"AuthMethod\":\"RPS\",\"SiteName\":\"user.auth.xboxlive.com\",\"RpsTicket\":\"d=" + token + "\"},\"RelyingParty\":\"http://auth.xboxlive.com\",\"TokenType\":\"JWT\"}";
            string jsonStr = await WebRequests.GetPostStringAsync(authUrl, authContent);
            Json.XboxToken json = JsonConvert.DeserializeObject<Json.XboxToken>(jsonStr);
            return json;
        }

        public async Task<Json.XboxToken> XSTSAuthenticateAsync(string token)
        {
            string authUrl = "https://xsts.auth.xboxlive.com/xsts/authorize";
            string authContent = "{\"Properties\":{\"SandboxId\":\"RETAIL\",\"UserTokens\":[\"" + token + "\"]},\"RelyingParty\":\"rp://api.minecraftservices.com/\",\"TokenType\":\"JWT\"}";
            string jsonStr = await WebRequests.GetPostStringAsync(authUrl, authContent);
            Json.XboxToken json = JsonConvert.DeserializeObject<Json.XboxToken>(jsonStr);
            return json;
        }

        public async Task<string> MinecraftAuthenticateAsync(Json.XboxToken token)
        {
            string authUrl = "https://api.minecraftservices.com/authentication/login_with_xbox";
            string authContent = "{\"identityToken\":\"XBL3.0 x=" + token.DisplayClaims.Xui[0].UserHash + ";" + token.Token + "\"}";
            string jsonStr = await WebRequests.GetPostStringAsync(authUrl, authContent);
            JObject json = JObject.Parse(jsonStr);
            return json.Value<string>("access_token");
        }

        public async Task<bool> CheckGameOwnershipAsync(string token)
        {
            string authUrl = "https://api.minecraftservices.com/entitlements/mcstore";
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", $"Bearer {token}");
            string jsonStr = await WebRequests.GetStringAsync(authUrl, headers);
            JObject json = JObject.Parse(jsonStr);
            var items = json.Value<JArray>("items");
            if (items.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Json.MinecraftProfileJson> GetProfileAsync(string token)
        {
            string authUrl = "https://api.minecraftservices.com/minecraft/profile";
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Authorization", $"Bearer {token}");
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

        public MicrosoftAuthenticator() { }
    }
}
