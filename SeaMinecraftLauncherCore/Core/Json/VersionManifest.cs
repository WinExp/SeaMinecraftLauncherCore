using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Json
{
    public class VersionManifest
    {
        public class LatestClass
        {
            [JsonProperty("release")]
            public readonly string Release;

            [JsonProperty("snapshot")]
            public readonly string Snapshot;
        }

        [JsonProperty("versions")]
        public readonly WebVersionInfo[] Versions;

        public WebVersionInfo FindLatestVersion(string latest)
        {
            foreach (var version in Versions)
            {
                if (version.Type == latest)
                {
                    return version;
                }
            }
            throw new NotImplementedException($"Latest \"{latest}\" not found.");
        }

        public WebVersionInfo FindVersion(string version)
        {
            foreach (var version_ in Versions)
            {
                if (version_.ID == version)
                {
                    return version_;
                }
            }
            throw new NotImplementedException($"Version {version} not found.");
        }
    }
}
