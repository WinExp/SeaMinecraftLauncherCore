using Microsoft.VisualBasic.Devices;
using System;

namespace SeaMinecraftLauncherCore.Tools
{
    public static class SystemHelper
    {
        public static double GetSystemTotalMemory()
        {
            ComputerInfo computerInfo = new ComputerInfo();
            return computerInfo.TotalPhysicalMemory;
        }

        public static double GetSystemMemoryOccupancy()
        {
            ComputerInfo computerInfo = new ComputerInfo();
            return (computerInfo.TotalPhysicalMemory - computerInfo.AvailablePhysicalMemory) / 1048576;
        }

        public static double GetSystemMemoryAvailable()
        {
            ComputerInfo computerInfo = new ComputerInfo();
            return computerInfo.AvailablePhysicalMemory / 1048576;
        }

        public static double DistributionMemory()
        {
            const double reverseMemory = 1024;
            double availableMemory = GetSystemMemoryAvailable();
            if (availableMemory - reverseMemory < reverseMemory)
            {
                if (availableMemory - reverseMemory <= 0)
                {
                    throw new OutOfMemoryException("内存不足。");
                }
                return availableMemory - reverseMemory;
            }
            return 4096;
        }
    }
}
