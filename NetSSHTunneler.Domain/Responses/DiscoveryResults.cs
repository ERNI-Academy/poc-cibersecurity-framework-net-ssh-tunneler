using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSHTunneler.Domain.Responses
{
    public class DiscoveryResults
    {
        public List<CommandResponse> commandResponses { get; set; }

        public DiscoveryResults()
        {
            commandResponses = new List<CommandResponse>();
        }
    }
}
