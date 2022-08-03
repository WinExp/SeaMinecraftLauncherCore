using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core
{
    public class MCVersion
    {
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Build { get; private set; }

        public int Prerelease { get; private set; }

        public int Snapshot_Year { get; private set; }
        public int Snapshot_Week { get; private set; }
        public int Snapshot_Fix { get; private set; }

        public VersionTypeEnum VersionType { get; private set; }

        public MCVersion(string version)
        {
            if (version[2] == 'w')
            {
                VersionType = VersionTypeEnum.Snapshot;
                Snapshot_Year = int.Parse(version.Substring(0, 2));
                Snapshot_Week = int.Parse(version.Substring(3, 2));
                Snapshot_Fix = version[6] - 64;
            }
            else
            {
                if (version.Contains("-pre"))
                {
                    int idx = version.IndexOf("-pre");
                    string[] versions = version.Split('.');
                    Major = int.Parse(versions[0]);
                    Minor = int.Parse(versions[1]);
                    Build = versions.Length == 3 ? int.Parse(versions[2].Remove(idx)) : 0;
                    VersionType = VersionTypeEnum.Prerelease;
                    Prerelease = int.Parse(version.Substring(idx + 4));
                }
                else
                {
                    VersionType = VersionTypeEnum.Release;
                    string[] versions = version.Split('.');
                    Major = int.Parse(versions[0]);
                    Minor = int.Parse(versions[1]);
                    Build = versions.Length == 3 ? int.Parse(versions[2]) : 0;
                }
            }
        }

        public enum VersionTypeEnum
        {
            Release,
            Prerelease,
            Snapshot
        }
    }
}
