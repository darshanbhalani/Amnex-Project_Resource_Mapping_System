namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class DashboardModal
    {
        public Graph Employees { get; set; }
        public Graph Projects { get; set; }
        public Graph Departments { get; set; }
        public Graph DepartmentProject { get; set; }
        public Graph DepartmentEmployee { get; set; }
        public Graph Inserts { get; set; }
        public Graph Updates { get; set; }
        public Graph Deletes { get; set; }
        public Graph PendingProjects { get; set; }
        public Graph RunningProjects { get; set; }
        public Graph CompletedProjects { get;set; }
        public Graph AllocatdeEmployees { get; set; }
        public Graph UnallocatedEmployees { get; set; }
    }
}
