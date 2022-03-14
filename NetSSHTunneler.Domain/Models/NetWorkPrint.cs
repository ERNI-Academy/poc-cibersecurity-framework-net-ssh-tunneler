using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSHTunneler.Services.Models
{
    public class NetWorkPrint
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public string Parent { get; set; }
        public List<NetWorkPrint> NetWork { get; set; }
        public NetWorkPrint()
        {
            NetWork = new List<NetWorkPrint>();
        }
         
    }
}
