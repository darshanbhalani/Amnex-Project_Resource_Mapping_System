using System;
using System.Collections.Generic;

namespace Amnex_Project_Resource_Mapping_System.Models;

public partial class Project
{
    public int Projectid { get; set; }

    public string Projectname { get; set; } = null!;

    public int Departmentid { get; set; }

    public DateOnly Startdate { get; set; }

    public DateOnly Enddate { get; set; }

    public string Skillsid { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Createdby { get; set; } = null!;

    public DateTime Createdtime { get; set; }

    public string Modifyby { get; set; } = null!;

    public DateTime Modifytime { get; set; }

    public bool Isdeleted { get; set; }

    public virtual Department Department { get; set; } = null!;
}
