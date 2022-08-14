using SeaMinecraftLauncherCore.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Model.Authentication
{
    public static class OfflineAuthenticator
    {
        public static OfflineUserInfo GetUserInfo(string username)
        {
            string uuid = GameHelper.GenerateOfflineUUID(username);
            OfflineUserInfo userInfo = new OfflineUserInfo
            {
                Username = username,
                UUID = uuid,
                AccessToken = uuid
            };
            return userInfo;
        }
    }

    public struct OfflineUserInfo
    {
        public string Username;
        public string UUID;
        public string AccessToken;
    }
}
