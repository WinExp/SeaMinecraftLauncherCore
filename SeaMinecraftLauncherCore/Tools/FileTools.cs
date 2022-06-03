using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Tools
{
    internal static class FileTools
    {
        internal static FileInfo[] SearchFile(string searchPath, string fileName)
        {
            List<FileInfo> files = new List<FileInfo>();
            try
            {
                foreach (var file in Directory.GetFiles(searchPath))
                {
                    if (Path.GetFileName(file) == fileName)
                    {
                        files.Add(new FileInfo(file));
                    }
                }
            }
            catch { }
            try
            {
                foreach (var directory in Directory.GetDirectories(searchPath))
                {
                    try
                    {
                        files.AddRange(SearchFile(directory, fileName));
                    }
                    catch { }
                }
            }
            catch { }

            return files.ToArray();
        }
    }
}
