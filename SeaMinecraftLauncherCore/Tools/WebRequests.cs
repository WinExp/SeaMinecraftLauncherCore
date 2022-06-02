using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Tools
{
    public static class WebRequests
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

        public static async Task<ChunkInfo[]> DownloadFileMultiThread(string url, string downloadPath, int chunkCount = 32)
        {
            ChunkInfo[] chunks = new ChunkInfo[chunkCount];
            Thread[] threads = new Thread[chunkCount];
            for (int i = 0; i < chunkCount - 1; i++)
            {
                threads[i] = new Thread(() =>
                {
                    chunks[i] = new ChunkInfo(i);
                    chunks[i].StartDownloadChunk(url, downloadPath, "temp", i).Wait();
                })
                {
                    IsBackground = true
                };
                threads[i].Start();
                threads[i].Join();
            }
            return chunks;
        }

        internal static async Task<long> GetContentLengthAsync(string url, int timeout = 10000)
        {
            using (var response = await GetRequestAsync(url, timeout))
            {
                return response.ContentLength;
            }
        }
    }

    public class ChunkInfo
    {
        public readonly int ChunkIndex;
        public long ChunkSize { get; private set; }
        public int Speed { get; private set; } = 0;
        public string TempFilePath { get; private set; }
        public string DownloadPath { get; private set; }
        public bool IsCompleted { get; private set; } = false;

        internal ChunkInfo(int index)
        {
            ChunkIndex = index;
        }

        internal async Task StartDownloadChunk(string url, string downloadPath, string tempPath, int chunkIndex)
        {
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(url);
            httpWebRequest.Method = "GET";
            long fileSize = await WebRequests.GetContentLengthAsync(url);
            ChunkSize = chunkIndex == 0 ? 0 : fileSize / chunkIndex;
            httpWebRequest.AddRange(ChunkSize * (chunkIndex - 1) + (chunkIndex - 1),
                fileSize - (ChunkSize * chunkIndex) < ChunkSize / 2
                ? ((ChunkSize * chunkIndex) + fileSize) - (ChunkSize * chunkIndex) : ChunkSize * chunkIndex);
            using (var response = await httpWebRequest.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    TempFilePath = Path.Combine(tempPath, Path.GetFileName(Path.GetTempFileName()));
                    DownloadPath = downloadPath;
                    using (FileStream fs = new FileStream(TempFilePath, FileMode.Create))
                    {
                        byte[] buffer = new byte[1024 * 1024 * 5];
                        int downloadedSize = 0;
                        Speed = stream.Read(buffer, 0, buffer.Length);
                        while (Speed > 0)
                        {
                            downloadedSize += Speed;
                            fs.Write(buffer, 0, downloadedSize);
                            Speed = stream.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
            IsCompleted = true;
        }
    }
}
