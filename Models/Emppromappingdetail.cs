namespace Amnex_Project_Resource_Mapping_System.Models;

public partial class Emppromappingdetail
{
    public int Projectid { get; set; }

    public int Employeeid { get; set; }

    public int? Projectrolesid { get; set; }

    public DateOnly Employeestartdate { get; set; }

    public DateOnly? Employeeenddate { get; set; }

    public bool Isactive { get; set; }

    public string? Createdby { get; set; }

    public DateTime? Createdtime { get; set; }

    public string? Modifyby { get; set; }

    public DateTime? Modifytime { get; set; }

    public string Logaction { get; set; } = null!;
}
