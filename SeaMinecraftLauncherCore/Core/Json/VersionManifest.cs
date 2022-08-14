using Newtonsoft.Json;
using System;

namespace SeaMinecraftLauncherCore.Core.Json
{
    public class VersionManifest
    {
        public class LatestClass
        {
            [JsonProperty("release")]
            public string Release;

            [JsonProperty("snapshot")]
            public string Snapshot;
        }

        [JsonProperty("versions")]
        public WebVersionInfo[] Versions;

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
