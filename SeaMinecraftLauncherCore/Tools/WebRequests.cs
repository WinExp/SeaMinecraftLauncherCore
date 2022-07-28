using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeaMinecraftLauncherCore.Tools
{
    public static class WebRequests
    {
        internal static async Task<HttpWebResponse> GetRequestAsync(string url, string type = "application/json", WebHeaderCollection headers = null, int timeout = 20000)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.Timeout = timeout;
            request.ContentType = type;
            request.Headers = headers ?? new WebHeaderCollection();
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        internal static async Task<string> GetStringAsync(string url, WebHeaderCollection headers = null, int timeout = 20000)
        {
            using (var response = await GetRequestAsync(url, headers: headers, timeout: timeout))
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        internal static async Task<HttpWebResponse> PostRequestAsync(string url, string content, string type = "application/json", int timeout = 20000)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "POST";
            request.Timeout = timeout;
            request.ContentType = type;
            request.Accept = type;
            using (var stream = request.GetRequestStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(content);
                stream.Write(buffer, 0, buffer.Length);
            }
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        internal static async Task<string> GetPostStringAsync(string url, string content, string type = "application/json", int timeout = 20000)
        {
            using (var response = await PostRequestAsync(url, content, type, timeout))
            {
                using (var stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
