using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetSSHTunneler.Domain.DTOs;
using NetSSHTunneler.Domain.Responses;
using NetSSHTunneler.Services.Interfaces;
using System.Text.Json;

namespace NetSSHTunneler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISshConnector _sshConnector;
        private readonly IFileOperations _fileOperations;
        public HomeController(ILogger<HomeController> logger, ISshConnector sshConnector, IFileOperations fileOperations)
        {
            _logger = logger;
            _sshConnector = sshConnector;
            _fileOperations = fileOperations;
        }

        [HttpPost("checkconnection")]
        public ConnectionStatusResponse CheckConnection([FromBody] SshConnectionDto sshConnection)
        {
            _logger.LogTrace("[SshController][CheckConnection] API called");
            var result = _sshConnector.CheckConnection(sshConnection);

            if (result.Status)
            {
                _logger.LogTrace("[SshController][CheckConnection] Save main_target_ip.json file");
                _fileOperations.WriteFile("main_target_ip.json", @".\configuration\", JsonSerializer.Serialize(sshConnection));
            }

            return result;
        }

        [HttpGet("checkdashboard")]
        public DashboardStatusResponse CheckDashboard()
        {
            _logger.LogTrace("[SshController][CheckDashboard] API called");
            var result = _fileOperations.ReadFile("main_target_ip.json", @".\configuration\");
            DashboardStatusResponse dashboardStatusResponse = new(result);
            return dashboardStatusResponse;
        }

        [HttpDelete("deletedashboard")]
        public void DeleteDashboardConfig()
        {
            _logger.LogTrace("[SshController][CheckDashboard] API called");
            _fileOperations.DeleteFile("main_target_ip.json");
        }
    }
}