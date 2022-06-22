using SeaMinecraftLauncherCore.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Model.Authentication
{
    public sealed class OfflineAuthenticator
    {
        public string Username { get; set; }

        public string UUID { get; set; }

        public string AccessToken { get; set; }

        public OfflineAuthenticator(string username)
        {
            Username = username;
            UUID = GameTools.GenerateOfflineUUID(username);
            AccessToken = UUID;
        }
    }
}
