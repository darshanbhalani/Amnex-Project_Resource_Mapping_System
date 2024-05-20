using NuGet.Packaging.Signing;

namespace Amnex_Project_Resource_Mapping_System.Models;

public partial class Project
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string Status { get; set; }
    public Timestamp ModifyTime { get; set; }
    public int ModifyById { get; set; }
    public int ModifyByName { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int CreatedById { get; set; }
    public string CreatedByName { get; set; }
    public Timestamp CreatedTime { get; set; }
    public bool isDeleted { get; set; }
    public string SkillName { get; set; }
    public string SkillId { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
}
