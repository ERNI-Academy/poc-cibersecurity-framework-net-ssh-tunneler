using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetSSHTunneler.Domain.DTOs;
using NetSSHTunneler.Domain.Responses;
using NetSSHTunneler.Services.Interfaces;
using NetSSHTunneler.Services.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace NetSSHTunneler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NetworkMapController : ControllerBase
    {
        private readonly ISshConnector _sshConnector;
        private readonly IFileOperations _fileOperations;
        private readonly INetworkOperations _networkOperations;
        public NetworkMapController(INetworkOperations networkOperations, ISshConnector sshConnector, IFileOperations fileOperations)
        {
            _sshConnector = sshConnector;
            _fileOperations = fileOperations;
            _networkOperations = networkOperations;
        }

        [HttpGet("getmap")]
        public List<NetWorkPrint> GetMap()
        {
            /* _logger.LogTrace("[SshController][CheckConnection] API called");
             var result = _sshConnector.CheckConnection(sshConnection);

             if (result.Status)
             {
                 _logger.LogTrace("[SshController][CheckConnection] Save main_target_ip.json file");
                 _fileOperations.WriteFile("main_target_ip.json", JsonSerializer.Serialize(sshConnection));
             }*/
            var result = _networkOperations.PrintNetMap();
            return result;
        }
        [HttpPost("getTarget")]
        public HostInfo GetTarget([FromBody] TargetDto IP)
        {
           var json = _fileOperations.FindAndReadConfigFile(IP.IP);
            var result = JsonSerializer.Deserialize<HostInfo>(json);
            return result;
        }
        [HttpPost("saveTargetConfig")]
        public bool SaveTargetConfig([FromBody] HostInfo HostInfo)
        {
            _fileOperations.SaveConfigFile(HostInfo);
            return true;
        }

        [HttpPost("saveGlobalConfig")]
        public bool SaveGlobalConfig([FromBody] GeneralConfiguration Configuration)
        {
            _fileOperations.SaveGlobalConfig(Configuration);
            return true;
        }

        [HttpGet("getGlobalConfig")]
        public GeneralConfiguration getGlobalConfig()
        {
            var file=_fileOperations.GetGlobalConfig();
            return file;
        }

        [HttpPost("saveDiscoveryConfig")]
        public bool SaveDiscoveryConfig([FromBody] DiscoveryConfiguration Configuration)
        {
            _fileOperations.SaveDiscoveryConfig(Configuration);
            return true;
        }

        [HttpPost("getDiscoveryConfig")]
        public DiscoveryConfiguration GetDiscoveryConfig([FromBody] GenericStringDTO config)
        {
            var file = _fileOperations.GetDiscoveryConfig(config.word);
            return file;
        }
        [HttpGet("getDiscoveryConfigList")]
        public List<DiscoveryConfiguration> GetDiscoveryConfigFiles()
        {
            var file = _fileOperations.GetDiscoveryConfigFiles(); 
            return file;
        }
    }
}