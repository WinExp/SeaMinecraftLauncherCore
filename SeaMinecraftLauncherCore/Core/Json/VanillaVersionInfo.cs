using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SeaMinecraftLauncherCore.Core.Json
{
    public class VanillaVersionInfo
    {
        [JsonIgnore]
        public readonly string[] OldVanillaJvmArgs =
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

        [JsonIgnore]
        public string VersionPath { get; internal set; }

        public class ArgumentsClass
        {
            [JsonIgnore]
            public string[] GameArgs { get; internal set; }

            [JsonIgnore]
            public string[] JvmArgs { get; internal set; }
        }

        public class AssetIndexClass
        {
            [JsonProperty("id")]
            public string ID;

            [JsonProperty("sha1")]
            public string SHA1;

            [JsonProperty("size")]
            public long Size;

            [JsonProperty("totalSize")]
            public string TotalSize;

            [JsonProperty("url")]
            public string URL;
        }


        public class DownloadsClass
        {
            public class ClientDownload
            {
                [JsonProperty("sha1")]
                public string SHA1;

                [JsonProperty("size")]
                public long Size;

                [JsonProperty("url")]
                public string URL;
            }

            [JsonProperty("client")]
            public ClientDownload Client;

            [JsonProperty("server")]
            public ClientDownload Server;
        }

        public class JavaTypeClass
        {
            [JsonProperty("component")]
            public string Component;

            [JsonProperty("majorVersion")]
            public int Major_Version;
        }

        public class LibrariesClass : IEquatable<LibrariesClass>
        {
            public class LibraryDownload : IEquatable<LibraryDownload>
            {
                public class Download : IEquatable<Download>
                {
                    [JsonProperty("path")]
                    public string Path;

                    [JsonProperty("id")]
                    public string ID;

                    [JsonProperty("sha1")]
                    public string SHA1;

                    [JsonProperty("size")]
                    public long Size;

                    [JsonProperty("url")]
                    public string URL;

                    public override bool Equals(object obj)
                    {
                        return Equals(obj as Download);
                    }

                    public bool Equals(Download other)
                    {
                        return Path == other?.Path
                            && ID == other?.ID
                            && SHA1 == other?.SHA1
                            && Size == other?.Size
                            && URL == other?.URL;
                    }

                    public static bool operator ==(Download d1, Download d2)
                    {
                        if (object.Equals(d1, null) && object.Equals(d2, null))
                        {
                            return true;
                        }
                        else if (object.Equals(d1, null) || object.Equals(d2, null))
                        {
                            return false;
                        }
                        return d1.Equals(d2);
                    }
                    public static bool operator != (Download d1, Download d2)
                    {
                        return !(d1 == d2);
                    }
                }

                public class ClassifiersClass : IEquatable<ClassifiersClass>
                {
                    [JsonProperty("natives-windows")]
                    public Download Windows;

                    [JsonProperty("natives-linux")]
                    public Download Linux;

                    [JsonProperty("natives-macos")]
                    public Download MacOS;

                    public override bool Equals(object obj)
                    {
                        return Equals(obj as ClassifiersClass);
                    }

                    public bool Equals(ClassifiersClass other)
                    {
                        return Windows == other?.Windows
                            && Linux == other?.Windows
                            && MacOS == other?.MacOS;
                    }

                    public static bool operator ==(ClassifiersClass c1, ClassifiersClass c2)
                    {
                        if (object.Equals(c1, null) && object.Equals(c2, null))
                        {
                            return true;
                        }
                        else if (object.Equals(c1, null) || object.Equals(c2, null))
                        {
                            return false;
                        }
                        return c1.Equals(c2);
                    }

                    public static bool operator != (ClassifiersClass c1, ClassifiersClass c2)
                    {
                        return !(c1 == c2);
                    }
                }

                [JsonProperty("artifact")]
                public Download Artifact;

                [JsonProperty("classifiers")]
                public ClassifiersClass Classifiers;

                public bool Equals(LibraryDownload other)
                {
                    return Artifact == other?.Artifact
                        && Classifiers == other.Classifiers;
                }

                public static bool operator ==(LibraryDownload l1, LibraryDownload l2)
                {
                    if (object.Equals(l1, null) && object.Equals(l2, null))
                    {
                        return true;
                    }
                    else if (object.Equals(l1, null) || object.Equals(l2, null))
                    {
                        return false;
                    }

                    return l1.Equals(l2);
                }

                public static bool operator !=(LibraryDownload l1, LibraryDownload l2)
                {
                    return !(l1 == l2);
                }
            }

            public class NativesClass : IEquatable<NativesClass>
            {
                [JsonProperty("natives-windows")]
                public string Windows;

                [JsonProperty("natives-linux")]
                public string Linux;

                [JsonProperty("natives-macos")]
                public string MacOS;

                public bool Equals(NativesClass other)
                {
                    return Windows == other?.Windows
                        && Linux == other?.Linux
                        && MacOS == other?.MacOS;
                }

                public static bool operator ==(NativesClass n1, NativesClass n2)
                {
                    if (object.Equals(n1, null) && object.Equals(n2, null))
                    {
                        return true;
                    }
                    else if (object.Equals(n1, null) || object.Equals(n2, null))
                    {
                        return false;
                    }
                    return n1.Equals(n2);
                }

                public static bool operator !=(NativesClass n1, NativesClass n2)
                {
                    return !(n1 == n2);
                }
            }

            public class Rule : IEquatable<Rule>
            {
                public class OSClass : IEquatable<OSClass>
                {
                    [JsonProperty("name")]
                    public string Name;

                    public bool Equals(OSClass other)
                    {
                        return Name == other?.Name;
                    }

                    public static bool operator ==(OSClass o1, OSClass o2)
                    {
                        if (object.Equals(o1, null) && object.Equals(o2, null))
                        {
                            return true;
                        }
                        else if (object.Equals(o2, null) || object.Equals(o2, null))
                        {
                            return false;
                        }
                        return o1.Equals(o2);
                    }

                    public static bool operator !=(OSClass o1, OSClass o2)
                    {
                        return !(o1 == o2);
                    }
                }

                [JsonProperty("action")]
                public string Action;

                [JsonProperty("os")]
                public OSClass OS;

                public bool Equals(Rule other)
                {
                    return Action == other?.Action
                        && OS == other?.OS;
                }

                public static bool operator ==(Rule r1, Rule r2)
                {
                    if (object.Equals(r1, null) && object.Equals(r2, null))
                    {
                        return true;
                    }
                    if (object.Equals(r1, null) || object.Equals(r2, null))
                    {
                        return false;
                    }
                    return r1.Equals(r2);
                }

                public static bool operator !=(Rule r1, Rule r2)
                {
                    return !(r1 == r2);
                }
            }

            [JsonProperty("downloads")]
            public LibraryDownload Download;

            [JsonProperty("name")]
            public string Name;

            [JsonProperty("natives")]
            public NativesClass Natives;

            [JsonProperty("rules")]
            public Rule[] Rules;

            public bool Equals(LibrariesClass other)
            {
                return Download == other.Download
                    && Name == other.Name
                    && Natives == other.Natives
                    && Rules == other.Rules;
            }

            public static bool operator ==(LibrariesClass l1, LibrariesClass l2)
            {
                if (object.Equals(l1, null) && object.Equals(l2, null))
                {
                    return true;
                }
                else if (object.Equals(l1, null) || object.Equals(l2, null))
                {
                    return false;
                }
                return l1.Equals(l2);
            }

            public static bool operator !=(LibrariesClass l1, LibrariesClass l2)
            {
                return !(l1 == l2);
            }
        }

        public class LoggingClass
        {
            public class Clientclass
            {
                public class Download
                {
                    [JsonProperty("id")]
                    public string ID;

                    [JsonProperty("sha1")]
                    public string SHA1;

                    [JsonProperty("size")]
                    public long Size;

                    [JsonProperty("url")]
                    public string URL;
                }

                [JsonProperty("argument")]
                public string Argument;

                [JsonProperty("file")]
                public Download File;

                [JsonProperty("type")]
                public string Type;
            }

            [JsonProperty("client")]
            public Clientclass Client;

        }

        [JsonProperty("arguments")]
        public ArgumentsClass Arguments;

        [JsonProperty("minecraftArguments")]
        public string OldMinecraftArguments;

        [JsonProperty("assetIndex", Required = Required.Always)]
        public AssetIndexClass AssetIndex;

        [JsonProperty("assets", Required = Required.Always)]
        public string Assets;

        [JsonProperty("complianceLevel")]
        public int ComplianceLevel;

        [JsonProperty("downloads", Required = Required.Always)]
        public DownloadsClass Downloads;

        [JsonProperty("id", Required = Required.Always)]
        public string ID;

        [JsonProperty("javaVersion")]
        public JavaTypeClass JavaType;

        [JsonProperty("libraries", Required = Required.Always)]
        public List<LibrariesClass> Libraries;

        [JsonProperty("logging", Required = Required.Always)]
        public LoggingClass Logging;

        [JsonProperty("mainClass", Required = Required.Always)]
        public string MainClass;

        [JsonProperty("minimumLauncherVersion")]
        public string MinLauncherVersion;

        [JsonProperty("releaseTime", Required = Required.Always)]
        public DateTime ReleaseTime;

        [JsonProperty("time")]
        public DateTime Time;

        [JsonProperty("type", Required = Required.Always)]
        public string Type;
    }
}
