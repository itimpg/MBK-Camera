using Mbk.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbk.Business.Interfaces
{
    public interface ICameraManager
    {
        Task<IList<Camera>> GetCameraListAsync();
        Task SaveCameraAsync(Camera camera);
        Task<string> GetCameraStatusAsync(string ipAddress);
        Task<string> CheckCameraInSystemAsync();
        Task<Camera> GetCameraAsync(int cameraId);
    }
}
