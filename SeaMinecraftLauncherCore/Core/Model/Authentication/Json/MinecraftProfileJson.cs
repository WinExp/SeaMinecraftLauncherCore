using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Model.Authentication.Json
{
    public class MinecraftProfileJson
    {
        public class SkinClass
        {
            [JsonProperty("id")]
            public string ID;

            [JsonProperty("state")]
            public string State;

            [JsonProperty("url")]
            public string Url;

            [JsonProperty("variant")]
            public string Variant;

            [JsonProperty("alias")]
            public string Alias;
        }

        [JsonProperty("id")]
        public string UUID;

        [JsonProperty("name")]
        public string Username;

        [JsonProperty("skins")]
        public SkinClass[] Skins;
    }
}
