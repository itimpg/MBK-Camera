using Mbk.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mbk.Business
{
    public class DataManager : IDataManager
    {
        public async Task<int> CollectDataAsync(string location)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(2000);
                return 20;
            });
        }
    }
}
