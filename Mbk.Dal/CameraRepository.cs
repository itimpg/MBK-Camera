using Mbk.Dal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mbk.Model;

namespace Mbk.Dal
{
    public class CameraRepository : ICameraRepository
    {
        public bool CheckConnection()
        {
            //using (var db = new MyEntityCollection())
            //{
            //    DbConnection conn = db.Database.Connection;
            //    try
            //    {
            //        conn.Open();   // check the database connection
            //        return true;
            //    }
            //    catch
            //    {
            //        return false;
            //    }
            //}
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Camera>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Camera> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(Camera camera)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Camera camera)
        {
            throw new NotImplementedException();
        }
    }
}
