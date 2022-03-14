using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSHTunneler.Services.Models
{
    public class CommandContainer
    {
        public List<string> Commands { get; set; }
        public bool Interactive { get; set; }
        public CommandConfig CommandConfig { get; set; }
        public CommandContainer()
        {
            Commands = new List<string>();
        }
    }
    public class CommandConfig
    {
        public int Timeout { get; set; }
        public Output Output { get; set; }
        public Crack Crack { get; set; }
        public Discovery Discovery { get; set; }
        public CommandConfig()
        {
            Output = new Output();
            Crack = new Crack();
            Discovery = new Discovery();
        }
    }
    public class Output
    {
        public bool SaveOutput { get; set; }
        public string Filename { get; set; }
    }
    public class Crack
    {
        public bool DoCrack { get; set; }
        public string HashFile { get; set; }
        public string Dictionary { get; set; }
    }
    public class Discovery
    {
        public bool RunDiscovery { get; set; }
        public string HostCommand { get; set; }
        public string NetworkCommand { get; set; }
        public string PortCommand { get; set; }
    }
}