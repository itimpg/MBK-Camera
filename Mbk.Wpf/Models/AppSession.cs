using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbk.Wpf.Models
{
    public class AppSessionModel
    {
        public int CameraId { get; set; }

        private AppSessionModel() { }

        private static AppSessionModel _session;
        public static AppSessionModel Instance()
        {
            return _session ?? (_session = new AppSessionModel());
        }
    }
}
