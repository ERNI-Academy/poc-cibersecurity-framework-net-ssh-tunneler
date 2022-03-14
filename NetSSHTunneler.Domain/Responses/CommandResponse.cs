using NetSSHTunneler.Domain.DTOs;
using System.Collections.Generic;

namespace NetSSHTunneler.Domain.Responses
{
    public class CommandResponse
    {
        public List<string> Results { get; set; }

        public bool Error { get; set; }

        public string Path { get; set; }
        public string Command { get; set; }
       
    }
}
