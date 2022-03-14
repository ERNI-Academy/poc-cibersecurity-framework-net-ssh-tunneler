namespace NetSSHTunneler.Domain.Responses
{
    public class ConnectionStatusResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string ConnectionName { get; set; }
    }
}
