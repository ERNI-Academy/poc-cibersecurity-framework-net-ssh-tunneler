using System.Collections.Generic;

namespace NetSSHTunneler.Domain.DTOs
{
    public class HostInfoDto
    {
        public string Network { get; set; }
        public string Parent { get; set; }
        public List<int> Ports { get; set; }
        public SshConnectionDto conectionInfo { get; set; }
        public HostInfoDto()
        {
            conectionInfo = new SshConnectionDto();
        }
    }
}
