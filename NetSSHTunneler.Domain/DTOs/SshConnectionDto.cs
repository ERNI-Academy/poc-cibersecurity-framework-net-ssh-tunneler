namespace NetSSHTunneler.Domain.DTOs
{

    public class SshConnectionDto
    {
        public string TargetIp { get; set; }
        public string TargetPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Certificate { get; set; }
        public string AdditionalSshParameters { get; set; }
    }
}
