using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenHardwareMonitor.Hardware;
using Microsoft.VisualBasic.Devices;
using Computer = OpenHardwareMonitor.Hardware.Computer;

namespace HW_Monitor_Server
{
    class HWMonitor
    {

        private Computer computer;
        private int cpu_count;

        [JsonProperty]
        private float[] cpu_usages;
        [JsonProperty(PropertyName = "cpu_temps")]
        private float[] cpu_temperatures;

        [JsonProperty]
        private long memory_available;
        private long memory_total;

        [JsonProperty]
        private float[] drive_usages;
        private DriveInfo[] drives;

        public HWMonitor()
        {
            this.computer = new Computer
            {
                CPUEnabled = true,
                GPUEnabled = true,
                RAMEnabled = true
            };
            computer.Open();
            this.cpu_count = _GetCPUCount();
            this.cpu_usages = new float[cpu_count];
            this.cpu_temperatures = new float[cpu_count];
            this.memory_total = (long) new ComputerInfo().TotalPhysicalMemory;
            this.drives = DriveInfo.GetDrives();
            this.drive_usages = new float[drives.Length];
        }

        public string GetStaticInfo()
        {
            drives = DriveInfo.GetDrives();
            Drive[] disk_partitions = new Drive[drives.Length];
            for (int i = 0; i < drives.Length; i++)
            {
                DriveInfo df = drives[i];
                disk_partitions[i] = new Drive(df.Name, df.DriveFormat, df.TotalSize);
            }
            return JsonConvert.SerializeObject(new
            {
                cpu_count = cpu_count - 1, // Minus one for CPU package
                memory_total = memory_total,
                disk_partitions = disk_partitions
            });
        }

        public string GetDynamicInfo()
        {
            // Get data from OpenHardwareMonitor
            foreach (var hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();
                foreach (IHardware subHardware in hardwareItem.SubHardware)
                {
                    subHardware.Update();
                }

                switch (hardwareItem.HardwareType)
                {
                    case HardwareType.CPU:
                        _UpdateCPU(hardwareItem);
                        break;
                }
            }

            // Update using data from Windows
            _UpdateRAM();
            _UpdateDrives();

            return GetAsJSON();
        }

        private int _GetCPUCount()
        {
            int cpu_count = 0;
            Computer computer = new Computer
            {
                CPUEnabled = true
            };
            computer.Open();
            foreach (var hardwareItem in computer.Hardware)
            {
                foreach (var sensor in hardwareItem.Sensors)
                    if (sensor.SensorType == SensorType.Load) cpu_count++;
            }
            return cpu_count;
        }

        private void _UpdateCPU(IHardware hardwareItem)
        {
            if (hardwareItem.HardwareType != HardwareType.CPU) return;
            foreach (var sensor in hardwareItem.Sensors)
            {
                if (sensor.SensorType == SensorType.Load)
                {
                    cpu_usages[sensor.Index] = sensor.Value ?? cpu_temperatures[sensor.Index];
                }
                else if (sensor.SensorType == SensorType.Temperature)
                {
                    cpu_temperatures[sensor.Index] = sensor.Value ?? cpu_temperatures[sensor.Index];
                }
            }
        }

        private void _UpdateRAM()
        {
            memory_available = (long) new ComputerInfo().AvailablePhysicalMemory;
        }

        private void _UpdateDrives()
        {
            drives = DriveInfo.GetDrives();
            if (drive_usages.Length == drives.Length)
            {
                for (int i = 0; i < drives.Length; i++)
                {
                    drive_usages[i] = drives[i].TotalSize - drives[i].TotalFreeSpace;
                }
            }
            else
            {
                throw new Exception("Drives changed - not implemented");
            }
        }

        public string GetAsJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        private class Drive
        {
            public readonly string device;
            public readonly string fstype;
            public readonly long capacity;
            public Drive(string device, string fstype, long capacity)
            {
                this.device = device;
                this.fstype = fstype;
                this.capacity = capacity;
            }
        }
    }
}
