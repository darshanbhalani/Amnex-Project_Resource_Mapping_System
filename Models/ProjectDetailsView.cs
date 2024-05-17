using Amnex_Project_Resource_Mapping_System.Models;

public class ProjectDetailsView
{
    public string ProjectName { get; set; }
    public int Progress { get; set; }
    public List<Employee> Employees { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}