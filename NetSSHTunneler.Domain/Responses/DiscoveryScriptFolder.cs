using NetSSHTunneler.Services.Models;
using System.Collections.Generic;

namespace NetSSHTunneler.Domain.Responses
{
    public class DiscoveryScriptFolder
    {
        public string Name { get; set; }
        public List<DiscoveryScriptFile> Scripts { get; set; }

    }

    public class DiscoveryScriptFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public CommandContainer Content { get; set; }
    }
}
