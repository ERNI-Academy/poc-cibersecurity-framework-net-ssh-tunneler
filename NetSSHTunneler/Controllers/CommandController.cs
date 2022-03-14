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
    public class CommandController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISshConnector _sshConnector;
        private readonly IFileOperations _fileOperations;
        public CommandController(ILogger<HomeController> logger, ISshConnector sshConnector, IFileOperations fileOperations)
        {
            _logger = logger;
            _sshConnector = sshConnector;
            _fileOperations = fileOperations;
        }

        [HttpPost("send")]
        public CommandResponse SendCommand([FromBody] CommandDto Command)
        {
            _logger.LogTrace("[CommandController][SendCommand] API called");
            var json = _fileOperations.FindAndReadConfigFile(Command.TargetIp);
            if (!string.IsNullOrEmpty(json))
            {
                var sshConnectionDto = JsonSerializer.Deserialize<HostInfoDto>(json);
                CommandContainer newCommand = new CommandContainer();
                newCommand.Commands.Add(Command.Command);
                newCommand.CommandConfig.Timeout = 200;
                CommandResponse result = _sshConnector.SendCommand(sshConnectionDto.conectionInfo, newCommand);
                return result;
            }
            else return null;
            
        }

        [HttpPost("processDiscovery")]
        public DiscoveryResults ProcessDiscovery([FromBody] ProcessDiscoveryDto Files)
        {
            _logger.LogTrace("[CommandController][ProcessDiscovery] API called");
            var json = _fileOperations.FindAndReadConfigFile(Files.TargetIp);
            var sshConnectionDto = JsonSerializer.Deserialize<HostInfoDto>(json);
            DiscoveryResults result = _sshConnector.ProcessDiscovery(sshConnectionDto.conectionInfo, Files.commands);
            return result;
        }
        [HttpGet("getDiscoveryScripts")]
        public List<DiscoveryScriptFolder> GetDiscoveryScripts()
        {
            _logger.LogTrace("[CommandController][GetDiscoveryScripts] API called");
            List<DiscoveryScriptFolder> result = _fileOperations.GetDiscoveryScripts();
            return result;
        }
        [HttpPost("redirectPort")]
        public bool RedirectPort([FromBody] PortRedirectionCommandDto command)
        {
            _logger.LogTrace("[CommandController][redirectPort] API called");
            var json = _fileOperations.FindAndReadConfigFile(command.TargetIp);
            var sshConnectionDto = JsonSerializer.Deserialize<HostInfoDto>(json);
            bool result= _sshConnector.RedirectPort(sshConnectionDto.conectionInfo, command.originIP, uint.Parse(command.originPort), uint.Parse(command.destinationPort), command.destinationIP);
            return result;
        }
        [HttpPost("saveDiscoveryScript")]
        public bool SaveDiscoveryFile([FromBody] DiscoveryScriptFile discoveryScriptFile)
        {
            _logger.LogTrace("[CommandController][saveDiscoveryScript] API called");
            var content = JsonSerializer.Serialize(discoveryScriptFile.Content);
            return _fileOperations.SaveDiscoveryFile(discoveryScriptFile.Path, content,discoveryScriptFile.Name);
        }

        [HttpPost("newDiscoveryScript")]
        public void NewDiscoveryScripts([FromBody] DiscoveryScriptFile discoveryScriptFile)
        {
            _logger.LogTrace("[CommandController][newDiscoveryScript] API called");
            var content = JsonSerializer.Serialize(discoveryScriptFile.Content);
            _fileOperations.SaveDiscoveryFile(discoveryScriptFile.Path, content, discoveryScriptFile.Name);
        }
    }
}