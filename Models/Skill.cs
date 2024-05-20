namespace Amnex_Project_Resource_Mapping_System.Models;

public partial class Skill
{
    public int Skillid { get; set; }

    public string Skillname { get; set; } = null!;

    public DateTime Createtime { get; set; }

    public string Createdby { get; set; } = null!;

    public string? Modifyby { get; set; }

    public DateTime Modifytime { get; set; }

    public bool Isdeleted { get; set; }
}
