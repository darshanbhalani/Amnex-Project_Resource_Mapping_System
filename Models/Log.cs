namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class Log
    {
        public string EntityName { get; set; }
        public string EntityType { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
        public DateTime CreateTime { get; set; }
        public string LogBy { get; set; }
    }
}
