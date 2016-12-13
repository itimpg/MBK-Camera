using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbk.Model
{
    public class HeatMap
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int Gmt { get; set; }
        public IList<HeatMapDetail> HeatMapDetails { get; set; }
    }

    public class HeatMapDetail
    {
        public int A { get; set; }
        public int B { get; set; }
    }
}
