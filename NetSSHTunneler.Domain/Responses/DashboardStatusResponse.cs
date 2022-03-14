using NetSSHTunneler.Domain.DTOs;

namespace NetSSHTunneler.Domain.Responses
{
    public class DashboardStatusResponse
    {
        public bool Configured { get; set; }
        public string Message { get; set; }
        public string TargetIp { get; set; }
        public string TargetPort { get; set; }
        public string UserName { get; set; }

        public DashboardStatusResponse(string json)
        {
            this.Configured = false;
            SshConnectionDto sshConnectionDto = null;

            if (json == null)
            {
                this.Message = "No configuration file found";
            }
            else
            {

                try
                {
                    sshConnectionDto = System.Text.Json.JsonSerializer.Deserialize<SshConnectionDto>(json);
                }
                catch
                {
                    this.Message = $"Error when trying to deserialize the file: {json}";
                }
            }

            if (sshConnectionDto != null)
            {
                this.TargetIp = sshConnectionDto.TargetIp;
                this.TargetPort = sshConnectionDto.TargetPort;
                this.UserName = sshConnectionDto.UserName;
                if (!string.IsNullOrEmpty(this.TargetIp))
                {
                    this.Configured = true;
                    this.Message = "Dashboard configured!";
                }
            }

        }
    }
}
