using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Tools
{
    public static class JavaTools
    {
        public static JavaInfo[] FindJava()
        {
            List<JavaInfo> javaList = new List<JavaInfo>();
            var rootReg = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32).OpenSubKey("SOFTWARE");
            if (rootReg == null)
            {
                return new JavaInfo[0];
            }

            try
            {
                var javaRootReg = rootReg.OpenSubKey("JavaSoft");
                var jreRootReg = javaRootReg.OpenSubKey("Java Runtime Environment");
                if (jreRootReg != null)
                {
                    foreach (string jre in jreRootReg.GetSubKeyNames())
                    {
                        string jreVersion = jre.Replace('_', '.');
                        var jreInfo = jreRootReg.OpenSubKey(jre);
                        javaList.Add(new JavaInfo(jreVersion, Path.Combine((string)jreInfo.GetValue("JavaHome"), "bin\\java.exe")));
                    }
                }

                var jdkRootReg = javaRootReg.OpenSubKey("JDK");
                if (jdkRootReg != null)
                {
                    foreach (string jdk in jdkRootReg.GetSubKeyNames())
                    {
                        string jdkVersion = jdk.Replace('_', '.');
                        var jdkInfo = jdkRootReg.OpenSubKey(jdk);
                        javaList.Add(new JavaInfo(jdkVersion, Path.Combine((string)jdkInfo.GetValue("JavaHome"), "bin\\java.exe")));
                    }
                }

                return javaList.ToArray();
            }
            catch
            {
                return new JavaInfo[0];
            }
        }
    }

    public class JavaInfo
    {
        public readonly Version JavaVersion;
        public readonly string JavaPath;

        internal JavaInfo(string version, string javaHome)
        {
            JavaVersion = Version.Parse(version);
            JavaPath = javaHome;
        }
    }
}
