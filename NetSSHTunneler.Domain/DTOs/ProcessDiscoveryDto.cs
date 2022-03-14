using System.Collections.Generic;

namespace NetSSHTunneler.Domain.DTOs
{
    public class ProcessDiscoveryDto
    {
        public string TargetIp { get; set; }
        public List<string> commands { get; set; }
    }
}
