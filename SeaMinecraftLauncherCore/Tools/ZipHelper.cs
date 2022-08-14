using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Tools
{
    internal static class ZipHelper
    {
        internal static int UnzipFile(string filePath, string unzipPath)
        {
            if (!Directory.Exists(unzipPath))
            {
                Directory.CreateDirectory(unzipPath);
            }
            int failedCount = 0;
            using (ZipInputStream input = new ZipInputStream(File.OpenRead(filePath)))
            {
                ZipEntry entry;
                while ((entry = input.GetNextEntry()) != null)
                {
                    try
                    {
                        string path = Path.Combine(unzipPath, Path.GetDirectoryName(entry.Name));
                        string fileName = Path.Combine(path, Path.GetFileName(entry.Name));
                        if (path != string.Empty && !Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        if (fileName != string.Empty)
                        {
                            using (FileStream stream = File.Create(fileName))
                            {
                                int size = 4096;
                                byte[] buffer = new byte[4096];
                                while (size > 0)
                                {
                                    size = input.Read(buffer, 0, buffer.Length);
                                    stream.Write(buffer, 0, buffer.Length);
                                }
                            }
                        }
                    }
                    catch
                    {
                        failedCount++;
                    }
                }
            }
            return failedCount;
        }
    }
}
