using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Json
{
    public class AssetsIndexInfo
    {
        public class FileClass : IEquatable<FileClass>
        {
            [JsonProperty("hash")]
            public string SHA1;

            [JsonProperty("size")]
            public int Size;

            public bool Equals(FileClass other)
            {
                return SHA1 == other.SHA1
                    && Size == other.Size;
            }
        }

        [JsonProperty("objects")]
        public Dictionary<string, FileClass> Assets = new Dictionary<string, FileClass>();

        public AssetsIndexInfo Clone()
        {
            AssetsIndexInfo assetsIndexInfo = new AssetsIndexInfo();
            foreach (var item in Assets)
            {
                assetsIndexInfo.Assets.Add(item.Key, item.Value);
            }
            return assetsIndexInfo;
        }
    }
}
