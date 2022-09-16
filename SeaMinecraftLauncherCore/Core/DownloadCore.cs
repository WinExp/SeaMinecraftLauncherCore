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

        public class DownloadProgress
        {
            public int CompletedCount;
            public int FailedCount;
            public int TotalCount;

            internal void StartCount(List<Task> asyncPool)
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        int completedCount = 0;
                        int failedCount = 0;
                        for (int i = 0; i < asyncPool.Count; i++)
                        {
                            Task task = asyncPool[i];
                            if (task == null)
                            {
                                continue;
                            }
                            if (task.IsCompleted)
                            {
                                completedCount++;
                            }
                            if (task.IsFaulted)
                            {
                                failedCount++;
                            }
                        }
                        CompletedCount = completedCount;
                        FailedCount = failedCount;
                        if (completedCount == asyncPool.Count)
                        {
                            break;
                        }
                        await Task.Delay(100);
                    }
                });
            }
        }

        static DownloadCore()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.MaxServicePoints = int.MaxValue;
        }

        public static DownloadProgress TryDownloadFiles(IEnumerable<DownloadInfo> downInfos, int retryCount = 2, int timeout = 15000)
        {
            DownloadProgress progress = new DownloadProgress();
            List<Task> asyncPool = new List<Task>();
            progress.TotalCount = downInfos.Count();
            progress.StartCount(asyncPool);
            //int openedThreadCount = 0;
            foreach (DownloadInfo downInfo in downInfos)
            {
                asyncPool.Add(Task.Run(async () =>
                {
                    //while (openedThreadCount >= 256) ;
                    //openedThreadCount++;
                    for (int i = 0; i < (retryCount == int.MaxValue ? retryCount : retryCount + 1); i++)
                    {
                        if (await TryDownloadFileAsync(downInfo, timeout))
                        {
                            goto SkipThrowException;
                        }
                    }
                    throw new NotImplementedException();
                SkipThrowException:
                    return;
                    //openedThreadCount--;
                }));
            }

            //await Task.WhenAll(asyncPool);
            return progress;
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
                    bool result = false;
                    webClient.DownloadFileCompleted += (s, e) =>
                    {
                        result = e.Error == null ? true : false;
                    };
                    var task = webClient.DownloadFileTaskAsync(downInfo.Url, Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt));
                    await Task.Delay(20000);
                    if (result)
                    {
                        if (File.Exists(Path.Combine(downInfo.DownloadPath, fileName)))
                        {
                            File.Delete(Path.Combine(downInfo.DownloadPath, fileName));
                        }
                        File.Move(Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt), Path.Combine(downInfo.DownloadPath, fileName));
                    }
                    else
                    {
                        webClient.CancelAsync();
                    }
                    /*
                    await Task.Delay(timeout);
                    if (task.IsCompleted)
                    {
                        if (result)
                        {
                            if (File.Exists(Path.Combine(downInfo.DownloadPath, fileName)))
                            {
                                File.Delete(Path.Combine(downInfo.DownloadPath, fileName));
                            }
                            File.Move(Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt), Path.Combine(downInfo.DownloadPath, fileName));
                        }
                    }
                    else
                    {
                        webClient.CancelAsync();
                        if (File.Exists(Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt)))
                        {
                            File.Delete(Path.Combine(downInfo.DownloadPath, fileNameWithOtherExt));
                        }
                    }
                    */
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
