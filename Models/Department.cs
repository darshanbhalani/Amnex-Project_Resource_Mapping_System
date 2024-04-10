using System;
using System.Collections.Generic;

namespace Amnex_Project_Resource_Mapping_System.Models;

public partial class Department
{
    public int Departmentid { get; set; }

    public string Departmentname { get; set; } = null!;

    public DateTime Createtime { get; set; }

    public string Createdby { get; set; } = null!;

    public string? Modifyby { get; set; }

    public DateTime Modifytime { get; set; }

    public bool Isdeleted { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
