using Mbk.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbk.Business.Interfaces
{
    public interface ICameraManager
    {
        Task<IList<CameraModel>> GetCameraListAsync();
        Task SaveCameraAsync(CameraModel camera);
        Task DeleteCameraAsync(int cameraId);
        Task<string> GetCameraStatusAsync(string ipAddress);
        Task<string> CheckCameraInSystemAsync();
        Task<CameraModel> GetCameraAsync(int cameraId);
    }
}
