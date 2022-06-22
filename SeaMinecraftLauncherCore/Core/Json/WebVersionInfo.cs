using Newtonsoft.Json;
using System;

namespace SeaMinecraftLauncherCore.Core.Json
{
    public class WebVersionInfo
    {
        [JsonProperty("id")]
        public string ID;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("url")]
        public string Url;

        [JsonProperty("time")]
        public DateTime Time;

        [JsonProperty("releaseTime")]
        public DateTime ReleaseTime;
    }
}
