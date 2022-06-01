using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Json
{
    public class WebVersionInfo
    {
        [JsonProperty("id")]
        public readonly string ID;

        [JsonProperty("type")]
        public readonly string Type;

        [JsonProperty("url")]
        public readonly string Url;

        [JsonProperty("time")]
        public readonly DateTime Time;

        [JsonProperty("releaseTime")]
        public readonly DateTime ReleaseTime;
    }
}
