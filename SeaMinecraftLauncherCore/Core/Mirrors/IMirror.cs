using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Mirrors
{
    public interface IMirror
    {
        public string Version_Manifest { get; }
        public string Version_Manifest_V2 { get; }
        public string Assets { get; }
        public string Libraries { get; }
        public string Mojang_Java { get; }
        public string Fabric_Meta { get; }
        public string Fabric_Maven { get; }
        public string Forge { get; }
        public string Liteloader { get; }
        public string Optifine { get; }
        public string Authlib_Injector { get; }
        public string Url_Launcher { get; }
        public string Url_LauncherMeta { get; }
    }

    public interface OptifineMirror
    {
        public string GetOptifineVersionList(string version);

        public string GetOptifineAllVersionList();
    }

    public class OptifineInfo
    {
        public string FileName { get; internal set; }
        public string MCVersion { get; internal set; }
    }
}
