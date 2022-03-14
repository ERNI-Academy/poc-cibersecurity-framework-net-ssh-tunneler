using System.Collections.Generic;

namespace NetSSHTunneler.Domain.DTOs
{
    public class PortRedirectionCommandDto
    {
        public string TargetIp { get; set; }
        public string originPort { get; set; }
        public string destinationPort { get; set; }
        public string originIP { get; set; }
        public string destinationIP { get; set; }
    }
}
