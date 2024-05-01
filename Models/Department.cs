namespace Amnex_Project_Resource_Mapping_System.Models;

public class Department
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public DateTime CreateTime { get; set; }
    public string CreatedBy { get; set; }
    public string ModifyBy { get; set; }
    public DateTime ModifyTime { get; set; }
    public Boolean IsDeleted { get; set; }
}
