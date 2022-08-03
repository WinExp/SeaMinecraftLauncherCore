using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Mirrors
{
    public class OfficialMirror : IMirror
    {
        public string Version_Manifest { get; } = "http://launchermeta.mojang.com/mc/game/version_manifest.json";
        public string Version_Manifest_V2 { get; } = "http://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
        public string Assets { get; } = "http://resources.download.minecraft.net/";
        public string Libraries { get; } = "https://libraries.minecraft.net/";
        public string Mojang_Java { get; } = "https://launchermeta.mojang.com/v1/products/java-runtime/2ec0cc96c44e5a76b9c8b7c39df7210883d12871/all.json";
        public string Fabric_Meta { get; } = "https://meta.fabricmc.net/";
        public string Fabric_Maven { get; } = "https://maven.fabricmc.net/";
        public string Forge { get; } = "https://files.minecraftforge.net/maven/";
        public string Liteloader { get; } = "http://dl.liteloader.com/versions/versions.json";
        public string Optifine { get; } = "";
        public string Authlib_Injector { get; } = "https://authlib-injector.yushi.moe";
        public string Url_Launcher { get; } = "https://launcher.mojang.com/";
        public string Url_LauncherMeta { get; } = "https://launchermeta.mojang.com/";
    }
}
