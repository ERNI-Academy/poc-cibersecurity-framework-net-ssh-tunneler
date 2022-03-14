using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSHTunneler.Services.Models
{
    public class DiscoveryConfiguration
    {
        public string configurationName { get; set; }
        public List<FileItem> files { get; set;}
        public DiscoveryConfiguration()
        {
            files = new List<FileItem>();
        }
    }
}
