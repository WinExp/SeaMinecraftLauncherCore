using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Json.Fabric
{
    public class Game
    {
        public GameClass[] Games { get; set; }

        public class GameClass
        {
            [JsonProperty("version", Required = Required.Always)]
            public string Version { get; set; }

            [JsonProperty("stable", Required = Required.Always)]
            public bool Stable { get; set; }
        }

        public GameClass FindVersion(string version)
        {
            foreach (var game in Games)
            {
                if (game.Version == version)
                {
                    return game;
                }
            }
            throw new NotImplementedException("未找到版本。");
        }
    }
}
