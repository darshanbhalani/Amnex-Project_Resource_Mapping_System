namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class LogGraph
    {
        public DateOnly Date { get; set; }
        public int Insert { get; set; }
        public int Update { get; set; }
        public int Delete { get; set; }
    }
}
