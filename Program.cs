using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HW_Monitor_Server;
using Newtonsoft.Json;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;

namespace HWMonitor_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            HWMonitor hwmonitor = new HWMonitor(6);
            while (true)
            {
                Console.WriteLine(hwmonitor.GetDynamicInfo());
                Console.WriteLine("==========================\n");
                System.Threading.Thread.Sleep(5000);
            }
            // Console.ReadLine();
        }

        static void Handle(IHardware hardwareItem)
        {
            Console.WriteLine(String.Format("==== {0} ====", hardwareItem.Name));
            Console.WriteLine("Subhardware: " + hardwareItem.SubHardware.Length);
            foreach (var sensor in hardwareItem.Sensors)
            {
                Console.WriteLine(String.Format("== Sensor: {0} ==", sensor.Name));
                Console.WriteLine(String.Format("Identifier: {0}", sensor.Identifier));
                Console.WriteLine(String.Format("Type: {0}", sensor.SensorType));
                Console.WriteLine(String.Format("Value: {0}", sensor.Value.HasValue ? sensor.Value.Value.ToString() : "no value", sensor.Index.ToString()));
            }
            Console.WriteLine("=============\n");
        }
    }

}
