using DesktopAPI.Models;

namespace DesktopAPI.Services
{
    public interface IPowershellService
    {
        Task Shutdown();
        Task<IEnumerable<DiskInfo>> GetDiskInfo(); 
    }
}