using System;
using System.Diagnostics;
using System.Management;

namespace SinticBolivia.License
{
    public class SBHardware
    {
        public SBHardware()
        {

        }
        public string GetCPUInfo()
        {
            Console.WriteLine(ManagementPath.DefaultPath.Path);
            //ManagementClass mc = new ManagementClass("");

            return "";
        }
    }
}

