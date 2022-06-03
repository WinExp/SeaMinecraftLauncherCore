using Microsoft.Win32;
using SeaMinecraftLauncherCore.Core.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SeaMinecraftLauncherCore.Tools
{
    public static class JavaTools
    {
        public static JavaInfo[] FindJava(bool deepScan = false)
        {
            List<JavaInfo> javaList = new List<JavaInfo>();
            var rootReg = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem
                ? RegistryView.Registry64 : RegistryView.Registry32).OpenSubKey("SOFTWARE");
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

                List<string> scanPaths = new List<string>();
                string[] deepScanPaths = { "Program Files\\Java", "Program Files (x86)\\Java", "MCLDownload\\ext" };
                if (deepScan)
                {
                    scanPaths.Add("");
                }
                else
                {
                    scanPaths.AddRange(deepScanPaths);
                }
                foreach (var disk in Directory.GetLogicalDrives())
                {
                    foreach (string path in scanPaths)
                    {
                        try
                        {
                            string scanPath = Path.Combine(disk, path);
                            DirectoryInfo directoryInfo = new DirectoryInfo(scanPath);
                            FileInfo[] fileInfos = directoryInfo.GetFiles("java.exe", SearchOption.AllDirectories);
                            foreach (FileInfo fileInfo in fileInfos)
                            {
                                try
                                {
                                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(fileInfo.FullName);
                                    JavaInfo javaInfo = new JavaInfo(fileVersionInfo.ProductName, fileInfo.FullName);
                                    foreach (var java in javaList)
                                    {
                                        if (java == javaInfo)
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

                return javaList.ToArray();
            }
            catch (Exception ex)
            {
                return new JavaInfo[0];
            }
        }

        public static JavaInfo AutoSelectJava(VanillaVersionInfo versionInfo, JavaInfo[] javaInfos)
        {
            Version dstJava = new Version(versionInfo.JavaType.Major_Version, 0);
            foreach (var javaInfo in javaInfos)
            {
                if (javaInfo.JavaVersion.Major == dstJava.Major)
                {
                    return javaInfo;
                }
            }
            throw new ArgumentException($"不存在 Major 版本为 {versionInfo.JavaType.Major_Version} 的 Java。");
        }
    }

    public class JavaInfo
    {
        public readonly Version JavaVersion;
        public readonly string JavaPath;

        internal JavaInfo(string version, string javaHome)
        {
            Version javaVersion = Version.Parse(version);
            if (javaVersion.Major == 1 && javaVersion.Minor == 8)
            {
                javaVersion = new Version(8, 0, javaVersion.Revision < 0 ? 0 : javaVersion.Revision);
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
            return java1.JavaVersion == java2.JavaVersion
                && java1.JavaPath == java2.JavaPath;
        }

        public static bool operator !=(JavaInfo java1, JavaInfo java2)
        {
            return !(java1 == java2);
        }
    }
}
