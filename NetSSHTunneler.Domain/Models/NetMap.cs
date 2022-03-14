using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSHTunneler.Services.Models
{
    public class Host
    {
        public string Parent { get; set; }
        public string NetworkName { get; set; }
        public string HostName { get; set; }
        public List<int> Ports { get; set; }
        public List<Network> Network { get; set; }

        public Host()
        {
            Ports = new List<int>();
            Network = new List<Network>();
        }

    }
    public class Network
    {
        public string NetworkName { get; set; }
        public List<Host> ChildHost { get; set; }

        public Network()
        {
            ChildHost = new List<Host>();
        }
    }
}
