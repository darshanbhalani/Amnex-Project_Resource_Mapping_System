namespace Amnex_Project_Resource_Mapping_System.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeUserName { get; set; }
    public string Email { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string SkillsId { get; set; }
    public string SkillsName { get; set; }
    public int EmployeeRating { get; set; }
    public string LoginRole { get; set; }
    public int LoginRoleId { get; set; }
    public bool IsAllocated { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int CreatedById { get; set; }
    public string CreatedbyName { get; set; }
    public DateTime CreatedTime { get; set; }
    public string ModifybyName { get; set; }
    public int ModifyById { get; set; }
    public DateTime ModifyTime { get; set; }
    public bool IsDeleted { get; set; }
    public string Password { get; set; }
}
