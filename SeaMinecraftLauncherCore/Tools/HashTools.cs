using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Tools
{
    internal static class HashTools
    {
        internal static string GetFileHash(string filePath, string algorithm)
        {
            using (var hash = HashAlgorithm.Create(algorithm))
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open))
                {
                    byte[] hashBuffer = hash.ComputeHash(file);
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (byte buf in hashBuffer)
                    {
                        stringBuilder.Append(buf.ToString("x2"));
                    }
                    return stringBuilder.ToString();
                }
            }
        }
    }
}
