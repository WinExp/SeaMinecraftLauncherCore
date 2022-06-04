using Microsoft.Win32;
using SeaMinecraftLauncherCore.Core.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SeaMinecraftLauncherCore.Tools
{
    public static class JavaTools
    {
        public static JavaInfo[] FindJava()
        {
            List<JavaInfo> javaList = new List<JavaInfo>();
            var rootReg = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem
                ? RegistryView.Registry64 : RegistryView.Registry32).OpenSubKey("SOFTWARE");
            if (rootReg == null)
            {
                return new JavaInfo[0];
            }


            RegistryKey javaRootReg;
            try
            {
                javaRootReg = rootReg.OpenSubKey("JavaSoft");
            }
            catch
            {
                return new JavaInfo[0];
            }
            try
            {
                // 在注册表中寻找 Java
                var jreRootReg = javaRootReg.OpenSubKey("Java Runtime Environment");
                if (jreRootReg != null)
                {
                    foreach (string jre in jreRootReg.GetSubKeyNames())
                    {
                        string jreVersion = jre.Replace('_', '.');
                        if (!jreVersion.Contains('.'))
                        {
                            jreVersion += ".0";
                        }
                        var jreInfo = jreRootReg.OpenSubKey(jre);
                        javaList.Add(new JavaInfo(jreVersion, Path.Combine((string)jreInfo.GetValue("JavaHome"), "bin\\java.exe")));
                    }
                }
            }
            catch { }

            try
            {
                var jdkRootReg = javaRootReg.OpenSubKey("JDK");
                if (jdkRootReg != null)
                {
                    foreach (string jdk in jdkRootReg.GetSubKeyNames())
                    {
                        string jdkVersion = jdk.Replace('_', '.');
                        if (!jdkVersion.Contains('.'))
                        {
                            jdkVersion += ".0";
                        }
                        var jdkInfo = jdkRootReg.OpenSubKey(jdk);
                        javaList.Add(new JavaInfo(jdkVersion, Path.Combine((string)jdkInfo.GetValue("JavaHome"), "bin\\java.exe")));
                    }
                }
            }
            catch { }

            try
            {
                // 在磁盘中按特定目录寻找 Java
                string[] searchList = { "Program Files\\Java", "Program Files (x86)\\Java", "Java", "MCLDownload\\ext", "ProgramData\\Oracle", "ProgramData\\BadlionClient" };
                foreach (string disk in Directory.GetLogicalDrives())
                {
                    foreach (string name in searchList)
                    {
                        try
                        {
                            foreach (var java in FileTools.SearchFile(Path.Combine(disk, name), "java.exe"))
                            {
                                try
                                {
                                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(java.FullName);
                                    JavaInfo javaInfo = new JavaInfo(fileVersionInfo.ProductVersion, java.FullName);
                                    foreach (var existJava in javaList)
                                    {
                                        if (javaInfo == existJava)
                                        {
                                            goto SkipJava;
                                        }
                                    }
                                    javaList.Add(javaInfo);
                                SkipJava:
                                    continue;
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            return javaList.ToArray();
        }

        public static JavaInfo AutoSelectJava(VanillaVersionInfo versionInfo, JavaInfo[] javaInfos)
        {
            JavaInfo selectJava = null;
            foreach (var javaInfo in javaInfos)
            {
                if (selectJava != null)
                {
                    if (javaInfo.JavaVersion > selectJava.JavaVersion && javaInfo.JavaVersion.Major == versionInfo.JavaType.Major_Version)
                    {
                        selectJava = javaInfo;
                    }
                }
                else if (javaInfo.JavaVersion.Major == versionInfo.JavaType.Major_Version)
                {
                    selectJava = javaInfo;
                }
            }
            if (selectJava == null)
            {
                throw new ArgumentException($"不存在 Major 版本为 {versionInfo.JavaType.Major_Version} 的 Java。");
            }
            return selectJava;
        }
    }

    public class JavaInfo
    {
        public readonly Version JavaVersion;
        public readonly string JavaPath;

        internal JavaInfo(string version, string javaHome)
        {
            Version javaVersion = Version.Parse(version);
            if (javaVersion.Major == 1)
            {
                javaVersion = new Version(javaVersion.Minor, 0, javaVersion.Revision < 0 ? 0 : javaVersion.Revision);
            }
            JavaVersion = javaVersion;
            JavaPath = javaHome;
        }

        public override bool Equals(object obj)
        {
            JavaInfo javaInfo = obj as JavaInfo;
            return this == javaInfo;
        }

        public static bool operator ==(JavaInfo java1, JavaInfo java2)
        {
            return java1?.JavaPath == java2?.JavaPath;
        }

        public static bool operator !=(JavaInfo java1, JavaInfo java2)
        {
            return !(java1 == java2);
        }
    }
}
