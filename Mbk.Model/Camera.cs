using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbk.Model
{
    public class Camera
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public string Floor { get; set; }
        public string CameraName { get; set; }
        public decimal Height { get; set; }
    }
}
