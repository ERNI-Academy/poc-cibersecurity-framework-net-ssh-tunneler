using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSHTunneler.Services.Models
{
    public class GeneralConfiguration
    {
        public string defaultTarget { get; set; }
        public string defaultDiscovery { get; set; }
        public int cracker { get; set; }
        public string crackerPath { get; set; }
    }
}
