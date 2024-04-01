namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class DBConfiguration
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        public string generateConnectionString()
        {
            return $"Host={this.Host};Port={this.Port};Username={this.Username};Password={this.Password};Database={this.Database}"; ;
        }

    }
}
