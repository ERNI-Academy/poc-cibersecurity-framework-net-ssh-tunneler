using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetSSHTunneler.Domain.DTOs;
using NetSSHTunneler.Domain.Responses;
using NetSSHTunneler.Services;
using NetSSHTunneler.Services.Interfaces;
using System;
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
        private readonly IEventService _chatHub;
        public HomeController(ILogger<HomeController> logger, ISshConnector sshConnector, IFileOperations fileOperations, IEventService chatHub)
        {
            _logger = logger;
            _sshConnector = sshConnector;
            _fileOperations = fileOperations;
            _chatHub = chatHub;
        }

        [HttpPost("checkconnection")]
        public ActionResult<ConnectionStatusResponse> CheckConnection([FromBody] SshConnectionDto sshConnection)
        {
            _logger.LogTrace("[SshController][CheckConnection] API called");

            if (sshConnection == null)
            {
                return new ObjectResult(new ProblemDetails { Status = 412, Title = "Error", Detail = "SSH Connection information no provided." });
            }

            if (string.IsNullOrEmpty(sshConnection.TargetIp))
            {
                return new ObjectResult(new ProblemDetails { Status = 412, Title = "Error", Detail = "Target Ip SSH Connection information no provided." });
            }

            var result = _sshConnector.CheckConnection(sshConnection);

            if (result.Status)
            {
                _logger.LogTrace("[SshController][CheckConnection] Save main_target_ip.json file");
                _fileOperations.WriteFile("main_target_ip.json", @".\configuration\", JsonSerializer.Serialize(sshConnection));
            }

            return this.Ok(result);
        }

        [HttpGet("checkdashboard")]
        public ActionResult<DashboardStatusResponse> CheckDashboard()
        {
            _logger.LogTrace("[SshController][CheckDashboard] API called");
            try
            {
                var result = _fileOperations.ReadFile("main_target_ip.json", @".\configuration\");
                DashboardStatusResponse dashboardStatusResponse = new(result);
                return this.Ok(dashboardStatusResponse);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ProblemDetails { Status = 500, Title = "Error reading main_target_ip.json", Detail = ex.ToString() });
            }
        }

        [HttpDelete("deletedashboard")]
        public ActionResult DeleteDashboardConfig()
        {
            _logger.LogTrace("[SshController][CheckDashboard] API called");
            try
            {
                _fileOperations.DeleteFile("main_target_ip.json");
                return this.Ok();
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ProblemDetails { Status = 500, Title = "Error deleting main_target_ip.json", Detail = ex.ToString() });
            }
        }

        [HttpGet("send")]
        public ActionResult SendMessage([FromQuery] string message)
        {
           _chatHub.SendMessage(new NewMessage("test", message, "Events"));
            return this.Ok();
        }
    }
}