using Microsoft.VisualBasic.FileIO;
using SeaMinecraftLauncherCore.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Core
{
    public class DownloadCore
    {
        internal class WebClientPlus : WebClient
        {
            internal int Timeout { get; set; }

            internal WebClientPlus(int timeout = 10000)
                => Timeout = timeout;

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                request.Timeout = Timeout;
                return request;
            }
        }

        public class DownloadInfo : IEquatable<DownloadInfo>
        {
            public string Url { get; set; }
            public string DownloadPath { get; set; }

            public string SHA1 { get; set; }

            public DownloadInfo(string url, string downPath, string sha1 = null)
            {
                Url = url;
                DownloadPath = Path.GetFullPath(downPath);
                SHA1 = sha1;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as DownloadInfo);
            }

            public bool Equals(DownloadInfo other)
            {
                return Url == other?.Url
                     && DownloadPath == other?.DownloadPath
                     && SHA1 == other?.SHA1;
            }

            public static bool operator ==(DownloadInfo d1, DownloadInfo d2)
            {
                if (Equals(d1, null) && Equals(d2, null))
                {
                    return true;
                }
                else if (Equals(d1, null) || Equals(d2, null))
                {
                    return false;
                }
                return d1.Equals(d2);
            }

            public static bool operator !=(DownloadInfo d1, DownloadInfo d2)
            {
                return !(d1 == d2);
            }
        }

        public class DownloadSuccessEventArgs : EventArgs
        {
            public DownloadInfo DownloadInfo { get; internal set; }

            public bool Success { get; internal set; }

            public DownloadSuccessEventArgs(DownloadInfo downloadInfo, bool success)
            {
                DownloadInfo = downloadInfo;
                Success = success;
            }
        }

        static DownloadCore()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.MaxServicePoints = int.MaxValue;
        }

        public static async Task TryDownloadFiles(IEnumerable<DownloadInfo> downInfos, int retryCount = 5, int threadCount = int.MaxValue, int timeout = 15000)
        {
            List<Task> asyncPool = new List<Task>();
            foreach (DownloadInfo downInfo in downInfos)
            {
                asyncPool.Add(Task.Run(async () =>
                {
                    for (int i = 0; i < (retryCount == int.MaxValue ? retryCount : retryCount + 1); i++)
                    {
                        if (await TryDownloadFileAsync(downInfo, timeout))
                        {
                            break;
                        }
                    }
                }));
            }

            await Task.WhenAll(asyncPool);
        }

        public static async Task<bool> TryDownloadFileAsync(DownloadInfo downInfo, int timeout = 10000)
        {
            string fileName = PathExtension.GetUrlFileName(downInfo.Url);
            string fileNameWithOtherExt = fileName + ".sml";
            try
            {
                using (var webClient = new WebClientPlus(timeout))
                {
                    if (!Directory.Exists(downInfo.DownloadPath))
                    {
                        Directory.CreateDirectory(downInfo.DownloadPath);
                    }
                    else if (File.Exists(Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt)))
                    {
                        File.Delete(Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt));
                    }
                    bool result = false;
                    webClient.DownloadFileCompleted += (s, e) =>
                    {
                        result = e.Error == null ? true : false;
                    };
                    var downTask = webClient.DownloadFileTaskAsync(downInfo.Url, Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt));
                    await Task.Delay(timeout);
                    if (downTask.IsCompleted)
                    {
                        if (File.Exists(Path.Combine(downInfo.DownloadPath, fileName)))
                        {
                            File.Delete(Path.Combine(downInfo.DownloadPath, fileName));
                        }
                        FileSystem.RenameFile(Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt), fileName);
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(downInfo.DownloadPath, fileName)))
                        {
                            File.Delete(Path.Combine(downInfo.DownloadPath, fileName));
                        }
                        if (File.Exists(Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt)))
                        {
                            File.Delete(Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt));
                        }
                        result = false;
                    }
                    return result;
                }
            }
            catch
            {
                if (File.Exists(Path.Combine(downInfo.DownloadPath, fileName)))
                {
                    File.Delete(Path.Combine(downInfo.DownloadPath, fileName));
                }
                if (File.Exists(Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt)))
                {
                    File.Delete(Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt));
                }
                return false;
            }
        }
    }
}
