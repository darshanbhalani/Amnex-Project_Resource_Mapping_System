using System;
using System.Collections.Generic;

namespace Amnex_Project_Resource_Mapping_System.Models;

public partial class Employee
{
    public int Employeeid { get; set; }

    public string Employeename { get; set; } = null!;

    public int Departmentid { get; set; }

    public string Skillsid { get; set; } = null!;

    public int Employeerating { get; set; }

    public dynamic Employeeroleid { get; set; }

    public bool Isallocated { get; set; }

    public int? Projectid { get; set; }

    public string? Projectrole { get; set; }

    public DateOnly? Startdate { get; set; }

    public DateOnly? Enddate { get; set; }

    public string Createdby { get; set; } = null!;

    public DateTime Createdtime { get; set; }

    public string Modifyby { get; set; } = null!;

    public DateTime Modifytime { get; set; }

    public bool Isdeleted { get; set; }

    public string Department { get; set; } 

    public string Email { get; set; }   
}
