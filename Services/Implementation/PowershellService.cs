using System;
using System.Management.Automation;
using System.Collections;
using DesktopAPI.Models;

namespace DesktopAPI.Services
{
    public class PowershellService : IPowershellService
    {
        public async Task<IEnumerable<DiskInfo>> GetDiskInfo()
        {
            var result = await RunScript("Get-CimInstance -ClassName Win32_LogicalDisk -Filter \"DriveType=3\"");

            var disks = new List<DiskInfo>();

            if(result != null)
            {
                foreach(var obj in result)
                {
                    var disk = new DiskInfo();
                    const int tbFactor = 1073741824;
                    disk.Letter = obj.Properties.Where(p => p.Name.Equals("Name")).FirstOrDefault().Value.ToString();
                    disk.Name = obj.Properties.Where(p => p.Name.Equals("VolumeName")).FirstOrDefault().Value.ToString();
                    disk.Size = Convert.ToSingle((ulong) obj.Properties.Where(p => p.Name.Equals("Size")).FirstOrDefault().Value / tbFactor);
                    disk.FreeSpace = Convert.ToSingle((ulong) obj.Properties.Where(p => p.Name.Equals("FreeSpace")).FirstOrDefault().Value / tbFactor);
                    disks.Add(disk);
                }
            }
            
            return disks;
        }

        public  Task Shutdown()
        {
            var result =  RunScript("Stop-Computer -ComputerName localhost -Force");
            return Task.FromResult(0);
        }

        private async Task<PSDataCollection<PSObject>?> RunScript(string script)
        {
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("Set-ExecutionPolicy Unrestricted -force");
                await ps.InvokeAsync();

                ps.AddScript(script);
                
                ps.Streams.Error.DataAdded += Error_DataAdded;
                ps.Streams.Warning.DataAdded += Warning_DataAdded;
                ps.Streams.Information.DataAdded += Information_DataAdded;

                var result = await ps.InvokeAsync().ConfigureAwait(false);

                ps.AddScript("Set-ExecutionPolicy Restricted -force");
                await ps.InvokeAsync();

                return result;
            }     
        }

        private void Information_DataAdded(object? sender, DataAddedEventArgs e)
        {
            var streamObjectsReceived = sender as PSDataCollection<InformationRecord>;
            if(streamObjectsReceived != null) {
                var currentStreamRecord = streamObjectsReceived[e.Index];
                Console.WriteLine($"InfoStreamEvent: {currentStreamRecord.MessageData}");
            }
        }

        private void Warning_DataAdded(object? sender, DataAddedEventArgs e)
        {
            var streamObjectsReceived = sender as PSDataCollection<WarningRecord>;
            if(streamObjectsReceived != null) {
                var currentStreamRecord = streamObjectsReceived[e.Index];
                Console.WriteLine($"WarningStreamEvent: {currentStreamRecord.Message}");
            }
        }

        private void Error_DataAdded(object? sender, DataAddedEventArgs e)
        {
            var streamObjectsReceived = sender as PSDataCollection<ErrorRecord>;
            if(streamObjectsReceived != null) {
                var currentStreamRecord = streamObjectsReceived[e.Index];
                Console.WriteLine($"ErrorStreamEvent: {currentStreamRecord.Exception}");
            }
        }
    }
}