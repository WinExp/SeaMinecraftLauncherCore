using System.Web;

namespace SeaMinecraftLauncherCore.Tools
{
    public static class PathExtension
    {
        public static string GetMinecraftRootPath(string anyMinecraftPath)
            => anyMinecraftPath.Substring(0, anyMinecraftPath.LastIndexOf(".minecraft") + 10);

        internal static string GetUrlFileName(string url)
        {
            int idx = url.IndexOf("?");
            return HttpUtility.UrlDecode((idx >= 0 ? url.Remove(idx) : url).Substring(url.LastIndexOf('/') + 1));
        }
    }
}
