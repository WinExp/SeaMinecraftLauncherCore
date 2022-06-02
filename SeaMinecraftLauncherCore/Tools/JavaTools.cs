using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

                string[] javaPaths = { "Program Files\\Java", "Program Files (x86)\\Java", "MCLDownload\\ext" };
                foreach (var disk in Directory.GetLogicalDrives())
                {
                    foreach (string javaPath in javaPaths)
                    {
                        try
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(disk, javaPath));
                            FileInfo[] fileInfos = directoryInfo.GetFiles("java.exe", SearchOption.AllDirectories);
                            foreach (FileInfo fileInfo in fileInfos)
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
                        }
                        catch
                        {
                            continue;
                        }
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
