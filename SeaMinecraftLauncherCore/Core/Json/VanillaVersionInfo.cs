using Newtonsoft.Json;
using System;

namespace SeaMinecraftLauncherCore.Core.Json
{
    public class VanillaVersionInfo
    {
        [JsonIgnore]
        public string VersionPath { get; internal set; }

        public class ArgumentsClass
        {
            [JsonIgnore]
            public string[] GameArgs { get; internal set; }

            [JsonIgnore]
            public readonly string[] JvmArgs =
            {
                "-XX:-UseAdaptiveSizePolicy",
                "-XX:-OmitStackTraceInFastThrow",
                "-Dfml.ignoreInvalidMinecraftCertificates=True",
                "-Dfml.ignorePatchDiscrepancies=True",
                "-Dlog4j2.formatMsgNoLookups=true",
                "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump",
                "-Djava.library.path=${natives_directory}",
                "-Dminecraft.launcher.brand=${launcher_name}",
                "-Dminecraft.launcher.version=${launcher_version}",
                "-cp",
                "${classpath}"
            };
        }

        public class AssetIndexClass
        {
            [JsonProperty("id")]
            public readonly string ID;

            [JsonProperty("sha1")]
            public readonly string SHA1;

            [JsonProperty("size")]
            public readonly long Size;

            [JsonProperty("totalSize")]
            public readonly string TotalSize;

            [JsonProperty("url")]
            public readonly string URL;
        }


        public class DownloadsClass
        {
            public class ClientDownload
            {
                [JsonProperty("sha1")]
                public readonly string SHA1;

                [JsonProperty("size")]
                public readonly long Size;

                [JsonProperty("url")]
                public readonly string URL;
            }

            [JsonProperty("client")]
            public readonly ClientDownload Client;

            [JsonProperty("server")]
            public readonly ClientDownload Server;
        }

        public class JavaTypeClass
        {
            [JsonProperty("component")]
            public readonly string Component;

            [JsonProperty("majorVersion")]
            public readonly int Major_Version;
        }

        public class LibrariesClass
        {
            public class LibraryDownload
            {
                public class Download
                {
                    [JsonProperty("path")]
                    public readonly string Path;

                    [JsonProperty("id")]
                    public readonly string ID;

                    [JsonProperty("sha1")]
                    public readonly string SHA1;

                    [JsonProperty("size")]
                    public readonly long Size;

                    [JsonProperty("url")]
                    public readonly string URL;
                }

                public class ClassifiersClass
                {
                    [JsonProperty("natives-windows")]
                    public readonly Download Windows;

                    [JsonProperty("natives-linux")]
                    public readonly Download Linux;

                    [JsonProperty("natives-macos")]
                    public readonly Download MacOS;
                }

                [JsonProperty("artifact")]
                public readonly Download Artifact;

                [JsonProperty("classifiers")]
                public readonly ClassifiersClass Classifiers;
            }

            public class NativesClass
            {
                [JsonProperty("natives-windows")]
                public readonly string Windows;

                [JsonProperty("natives-linux")]
                public readonly string Linux;

                [JsonProperty("natives-macos")]
                public readonly string MacOS;
            }

            public class Rule
            {
                public class OSClass
                {
                    [JsonProperty("name")]
                    public readonly string Name;
                }

                [JsonProperty("action")]
                public readonly string Action;

                [JsonProperty("os")]
                public readonly OSClass OS;
            }

            [JsonProperty("downloads")]
            public readonly LibraryDownload Download;

            [JsonProperty("name")]
            public readonly string Name;

            [JsonProperty("natives")]
            public readonly NativesClass Natives;

            [JsonProperty("rules")]
            public readonly Rule[] Rules;
        }

        public class LoggingClass
        {
            public class Clientclass
            {
                public class Download
                {
                    [JsonProperty("id")]
                    public readonly string ID;

                    [JsonProperty("sha1")]
                    public readonly string SHA1;

                    [JsonProperty("size")]
                    public readonly long Size;

                    [JsonProperty("url")]
                    public readonly string URL;
                }

                [JsonProperty("argument")]
                public readonly string Argument;

                [JsonProperty("file")]
                public readonly Download File;

                [JsonProperty("type")]
                public readonly string Type;
            }

            [JsonProperty("client")]
            public readonly Clientclass Client;

        }

        [JsonProperty("arguments")]
        public readonly ArgumentsClass Arguments;

        [JsonProperty("minecraftArguments")]
        public readonly string OldMinecraftArguments;

        [JsonProperty("assetIndex")]
        public readonly AssetIndexClass AssetIndex;

        [JsonProperty("assets")]
        public readonly string Assets;

        [JsonProperty("complianceLevel")]
        public readonly int ComplianceLevel;

        [JsonProperty("downloads")]
        public readonly DownloadsClass Downloads;

        [JsonProperty("id")]
        public readonly string ID;

        [JsonProperty("javaVersion")]
        public readonly JavaTypeClass JavaType;

        [JsonProperty("libraries")]
        public readonly LibrariesClass[] Libraries;

        [JsonProperty("logging")]
        public readonly LoggingClass Logging;

        [JsonProperty("mainClass")]
        public readonly string MainClass;

        [JsonProperty("minimumLauncherVersion")]
        public readonly string MinLauncherVersion;

        [JsonProperty("releaseTime")]
        public readonly DateTime ReleaseTime;

        [JsonProperty("time")]
        public readonly DateTime Time;

        [JsonProperty("type")]
        public readonly string Type;

        [JsonProperty("clientVersion")]
        public readonly string ClientVersion;
    }
}
