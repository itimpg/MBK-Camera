using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mbk.Model;
using System.Data.Common;
using AutoMapper;
using Mbk.Dal.Repositories.Interfaces;

namespace Mbk.Dal.Repositories
{
    public class CameraRepository : BaseRepository, ICameraRepository
    {
        public CameraRepository(string connectionString)
            : base(connectionString)
        {
        }

        public bool CheckConnection()
        {
            using (var db = new MbkCameraDb(ConnectionString))
            {
                DbConnection conn = db.Database.Connection;
                try
                {
                    conn.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var db = new MbkCameraDb(ConnectionString))
            {
                var camera = db.Cameras.FirstOrDefault(x => x.Id == id);
                db.Cameras.Remove(camera);
                await db.SaveChangesAsync();
            }
        }

        public async Task<IList<CameraModel>> GetAsync()
        {
            return await Task.Run(() =>
            {
                using (var db = new MbkCameraDb(ConnectionString))
                {
                    var cameras = db.Cameras.ToList();
                    return Mapper.Map<List<CameraModel>>(cameras);
                }
            });
        }

        public async Task<CameraModel> GetAsync(int id)
        {
            return await Task.Run(() =>
            {
                using (var db = new MbkCameraDb(ConnectionString))
                {
                    var camera = db.Cameras.FirstOrDefault(x => x.Id == id);
                    return Mapper.Map<CameraModel>(camera);
                }
            });
        }

        public async Task InsertAsync(CameraModel model)
        {
            using (var db = new MbkCameraDb(ConnectionString))
            {
                var camera = Mapper.Map<Camera>(model);
                db.Cameras.Add(camera);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(CameraModel model)
        {
            using (var db = new MbkCameraDb(ConnectionString))
            {
                var camera = db.Cameras.FirstOrDefault(x => x.Id == model.Id);
                if (camera != null)
                {
                    camera.IpAddress = model.IpAddress;
                    camera.Floor = model.Floor;
                    camera.Name = model.Name;
                    camera.Height = model.Height;
                    camera.Username = model.Username;
                    camera.Password = model.Password;

                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
