using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Model.Authentication.Json
{
    public class MicrosoftJson
    {
        [JsonProperty("access_token", Required = Required.Always)]
        public string Access_Token;

        [JsonProperty("refresh_token", Required = Required.Always)]
        public string Refresh_Token;
    }
}
