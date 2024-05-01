using NuGet.Packaging.Signing;

namespace Amnex_Project_Resource_Mapping_System.Models;

public partial class Project
{
    public int projectId { get; set; }
    public string projectName { get; set; }
    public string status { get; set; }
    public Timestamp modifyTime { get; set; }
    public int modifyBy { get; set; }

    public DateOnly startDate { get; set; }
    public DateOnly endDate { get; set; }

    public int createdBy { get; set; }

    public Timestamp createdTime { get; set; }
    public bool isDeleted { get; set; }




    //skills
    public string skillname { get; set; }
    public string skillid { get; set; }




    //department
    public int departmentId { get; set; }
    public string departmentName { get; set; }




    //Employee
    public string employeeName { get; set; }
}
