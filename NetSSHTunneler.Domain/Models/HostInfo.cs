using NetSSHTunneler.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSHTunneler.Services.Models
{
    public class HostInfo
    {
        public string Network { get; set; }
        public string Parent { get; set; }
        public List<int> Ports { get; set; }
        public SshConnectionDto conectionInfo { get; set; }
        public HostInfo()
        {
            conectionInfo = new SshConnectionDto();
        }
    }
}
