using Downloader;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Tools
{
    internal static class WebRequests
    {
        internal static async Task<HttpWebResponse> GetRequestAsync(string url, int timeout = 10000)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.Timeout = timeout;
            return (HttpWebResponse)await request.GetResponseAsync();
        }
        internal static async Task<string> GetStringAsync(string url, int timeout = 10000)
        {
            using (var response = await GetRequestAsync(url, timeout))
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        internal static async Task<DownloadInfo> DownloadFile(string url, string downloadPath,
            IWebProxy proxy = null, int timeout = 10000)
        {
            long fileLength = await GetDownloadFileSize(url, timeout);
            int chunkCount = (int)Math.Ceiling((double)fileLength / 8388608);
            DownloadConfiguration downloadConfig = new DownloadConfiguration
            {
                BufferBlockSize = 10240,
                ChunkCount = chunkCount,
                Timeout = timeout,
                MaxTryAgainOnFailover = int.MaxValue,
                OnTheFlyDownload = false,
                ParallelDownload = true,
                RequestConfiguration =
                {
                    Proxy = proxy
                }
            };
            DownloadService downloader = new DownloadService(downloadConfig);
            downloader.DownloadFileTaskAsync(url, new DirectoryInfo(downloadPath));
            return new DownloadInfo(downloader);
        }

        internal static async Task<long> GetDownloadFileSize(string url, int timeout = 10000)
        {
            using (var response = await GetRequestAsync(url, timeout))
            {
                return response.ContentLength;
            }
        }
    }

    public class DownloadInfo
    {
        public string FileName { get; private set; }
        public string FileExtension { get; private set; }
        public string DownloadPath { get; private set; }
        public long TotalFileLength { get; private set; }
        public long ProgressedDataLength { get; private set; }
        public double DownloadedDataPercent { get; private set; }
        public double DownloadSpeed { get; private set; }
        public double DownloadSpeedAverage { get; private set; }
        public int ChunkCount { get; private set; }
        public bool IsCompleted { get; private set; }

        internal DownloadInfo(DownloadService downloader)
        {
            downloader.DownloadStarted += DownloadStarted;
            downloader.DownloadProgressChanged += DownloadProgressChanged;
            downloader.DownloadFileCompleted += DownloadFileCompleted;
            ChunkCount = downloader.Package.Chunks.Length;
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            IsCompleted = true;
        }

        private void DownloadProgressChanged(object sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            ProgressedDataLength = e.ProgressedByteSize;
            DownloadedDataPercent = e.ProgressPercentage;
            DownloadSpeed = e.BytesPerSecondSpeed;
            DownloadSpeedAverage = e.AverageBytesPerSecondSpeed;
        }

        private void DownloadStarted(object sender, DownloadStartedEventArgs e)
        {
            FileName = Path.GetFileName(e.FileName);
            FileExtension = Path.GetExtension(e.FileName);
            DownloadPath = Path.GetDirectoryName(e.FileName);
            TotalFileLength = e.TotalBytesToReceive;
            IsCompleted = false;
        }

        public void Wait()
        {
            while (!IsCompleted) { }
        }
    }
}
