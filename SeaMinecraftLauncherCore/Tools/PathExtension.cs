namespace SeaMinecraftLauncherCore.Tools
{
    internal static class PathExtension
    {
        internal static string GetMinecraftRootPath(string anyMinecraftPath)
        {
            return anyMinecraftPath.Substring(0, anyMinecraftPath.LastIndexOf(".minecraft") + 10);
        }
    }
}
