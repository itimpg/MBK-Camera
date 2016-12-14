using Mbk.Business.Interfaces;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Mbk.Business
{
    public class CameraManager : ICameraManager
    {
        private ICameraRepository _cameraRepository;

        public CameraManager(ICameraRepository cameraRepository)
        {
            _cameraRepository = cameraRepository;
        }

        public async Task<string> GetCameraStatusAsync(string ipAddress)
        {
            return await Task.Run(() =>
            {
                Ping pingSender = new Ping();
                IPAddress address;
                bool isValidIpAddress = IPAddress.TryParse(ipAddress, out address);
                if (!isValidIpAddress)
                {
                    return $"Invalid IPAddress";
                }
                else
                {
                    PingReply reply = pingSender.Send(address);
                    return reply.Status == IPStatus.Success ? "Connected" : "Disconnected";
                }
            });
        }

        public async Task<string> CheckCameraInSystemAsync()
        {
            bool isDbConnected = _cameraRepository.CheckConnection();
            if (isDbConnected)
            {
                int connectedCamera = 0;
                var cameras = await GetCameraListAsync();
                await Task.WhenAll(cameras.Select(async camera =>
                {
                    string status = await GetCameraStatusAsync(camera.IpAddress);
                    if (status == "Connected")
                    {
                        connectedCamera += 1;
                    }
                }));

                return $"DB connected, {connectedCamera} camera(s) connected";
            }
            else
            {
                return "DB not connected, cannot get data from camera";
            }
        }

        public async Task<CameraModel> GetCameraAsync(int cameraId)
        {
            return await _cameraRepository.GetAsync(cameraId);
        }

        public async Task<IList<CameraModel>> GetCameraListAsync()
        {
            return await _cameraRepository.GetAsync();
        }

        public async Task SaveCameraAsync(CameraModel camera)
        {
            if (camera.Id == 0)
            {
                await _cameraRepository.InsertAsync(camera);
            }
            else
            {
                await _cameraRepository.UpdateAsync(camera);
            }
        }
    }
}
