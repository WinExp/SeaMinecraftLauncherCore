using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core.Installer
{
    public interface IModLoaderInstaller
    {
        public InstallProgress InstallVersion(string mcversion, string loaderVersion);
    }
}
