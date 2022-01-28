using Nefarius.ViGEm.Client;
using System;

namespace DS4Windows
{
    static class DS4OutDeviceFactory
    {
        private static readonly Version extAPIMinVersion = new("1.17.333.0");

        public static DS4OutDevice CreateDS4Device(ViGEmClient client,
            Version driverVersion)
        {
            DS4OutDevice result = null;
            if (extAPIMinVersion.CompareTo(driverVersion) <= 0)
            {
                result = new DS4OutDeviceExt(client);
            }
            else
            {
                result = new DS4OutDeviceBasic(client);
            }

            return result;
        }
    }
}
