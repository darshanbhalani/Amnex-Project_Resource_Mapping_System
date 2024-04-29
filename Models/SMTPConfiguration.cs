namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class SMTPConfiguration
    {
        public string Server { get; set; }
        public string HostName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public bool EnableSSL { get; set; } = true;
    }
}
