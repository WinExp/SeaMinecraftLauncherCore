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
