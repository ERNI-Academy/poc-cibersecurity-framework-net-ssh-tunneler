namespace NetSSHTunneler.Domain.DTOs
{
    public class CommandDto
    {
        public string AttackedIp { get; set; }
        public string TargetIp { get; set; }
        public string TargetPort { get; set; }
        public string Command { get; set; }

    }
}
