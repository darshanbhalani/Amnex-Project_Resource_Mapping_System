using System;
using System.Collections.Generic;

namespace Amnex_Project_Resource_Mapping_System.Models;

public partial class Employee
{
    public int Employeeid { get; set; }
    public string Employeename { get; set; }
    public string EmployeeUserName { get; set; }
    public string Email { get; set; }
    public int Departmentid { get; set; }
    public string DepartmentName { get; set; }
    public string SkillsId { get; set; }
    public string SkillsName { get; set; }
    public int EmployeeRating { get; set; }
    public string LoginRole { get; set; }
    public int LoginRoleId { get; set; }
    public bool Isallocated { get; set; }
    public DateOnly? Startdate { get; set; }
    public DateOnly? Enddate { get; set; }
    public int CreatedById { get; set; }
    public string CreatedbyName { get; set; }
    public DateTime Createdtime { get; set; }
    public string ModifybyName { get; set; }
    public int ModifyById { get; set; }
    public DateTime Modifytime { get; set; }
    public bool Isdeleted { get; set; }
    public string Password {  get; set; }
}
