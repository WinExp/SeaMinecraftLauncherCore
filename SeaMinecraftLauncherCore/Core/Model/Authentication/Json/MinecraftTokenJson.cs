using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Model.Authentication.Json
{
    public class MinecraftTokenJson
    {
        [JsonProperty("access_token", Required = Required.Always)]
        public string AccessToken;

        [JsonProperty("expires_in", Required = Required.Always)]
        public int Expires;
    }
}
