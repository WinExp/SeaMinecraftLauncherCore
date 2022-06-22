using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Model.Authentication.Json
{
    public class XboxToken
    {
        public class DisplayClaimsClass
        {
            public class XuiClass
            {
                [JsonProperty("uhs", Required = Required.Always)]
                public string UserHash;
            }

            [JsonProperty("xui", Required = Required.Always)]
            public XuiClass[] Xui;
        }

        [JsonProperty(Required = Required.Always)]
        public string Token;

        [JsonProperty("DisplayClaims", Required = Required.Always)]
        public DisplayClaimsClass DisplayClaims;
    }
}
