namespace Amnex_Project_Resource_Mapping_System.Models;

public class Department
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public DateTime CreateTime { get; set; }
    public int CreatedBy { get; set; }
    public int ModifyBy { get; set; }
    public DateTime ModifyTime { get; set; }
    public Boolean IsDeleted { get; set; }
    public int TotalProjects { get; set; }
    public int PendingProjects { get; set; }
    public int RunningProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int TotalEmployees { get; set; }
    public int AllocatedEmployees { get; set; }
    public int UnallocatedEmployees { get; set; }
}
