using DaanV2.UUID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Tools
{
    public static class GameTools
    {
        public static string GenerateOfflineUUID(string username)
        {
            return UUIDFactory.CreateUUID(3, 1, $"OfflinePlayer:{username}").ToString().Replace("-", "");
        }
    }
}
