using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Management;

namespace VinXiangQi
{
    public class MachineCode
    {

        static MachineCode machineCode;

        public static string GetMachineCodeString()
        {
            string machineCodeString = string.Empty;
            if (machineCode == null)
            {
                machineCode = new MachineCode();
            }
            machineCodeString = "PC." + machineCode.GetCpuInfo() + "." +
                                machineCode.GetHDid() + "." +
                                machineCode.GetMoAddress();
            //machineCodeString = machineCode.GetMoAddress();
            return machineCodeString;
        }

        ///   <summary> 
        ///   获取cpu序列号     
        ///   </summary> 
        ///   <returns> string </returns> 
        public string GetCpuInfo()
        {
            string cpuInfo = "";
            try
            {
                using (ManagementClass cimobject = new ManagementClass("Win32_Processor"))
                {
                    ManagementObjectCollection moc = cimobject.GetInstances();

                    foreach (ManagementObject mo in moc)
                    {
                        cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                        mo.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return cpuInfo.ToString();
        }

        ///   <summary> 
        ///   获取硬盘ID     
        ///   </summary> 
        ///   <returns> string </returns> 
        public string GetHDid()
        {
            string HDid = "";
            try
            {
                using (ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive"))
                {
                    ManagementObjectCollection moc1 = cimobject1.GetInstances();
                    foreach (ManagementObject mo in moc1)
                    {
                        HDid = (string)mo.Properties["Model"].Value;
                        mo.Dispose();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return HDid.ToString();
        }

        ///   <summary> 
        ///   获取网卡硬件地址 
        ///   </summary> 
        ///   <returns> string </returns> 
        public string GetMoAddress()
        {
            string MoAddress = "";
            try
            {
                using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                {
                    ManagementObjectCollection moc2 = mc.GetInstances();
                    foreach (ManagementObject mo in moc2)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                            MoAddress = mo["MacAddress"].ToString();
                        mo.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return MoAddress.ToString();
        }
    }
    public class StatisticsHelper
    {
        public static async void Statistics()
        {
            await Task.Run(new Action(() =>
            {
                try
                {
                    WebClient wc = new WebClient();
                    Debug.WriteLine($"Running on {MachineCode.GetMachineCodeString()}\nVersion {Mainform.Version}");
                    wc.DownloadString("http://xq.vcccz.com/statistics.php?machine=" + MachineCode.GetMachineCodeString() + "&version=" + Mainform.Version);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }));
        }
    }
}
