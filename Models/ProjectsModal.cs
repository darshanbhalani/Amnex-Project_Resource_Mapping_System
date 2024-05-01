namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class ProjectsModal
    {
        public List<Project> Projects { get; set; }
        public List<dynamic> Skills { get; set; }
        public List<dynamic> Departments { get; set; }
        public int RunningProjects { get; set; }
        public int CompletedProjects { get; set; }
    }
}
