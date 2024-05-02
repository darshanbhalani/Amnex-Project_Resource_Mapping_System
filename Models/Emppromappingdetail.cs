namespace Amnex_Project_Resource_Mapping_System.Models;

public partial class Emppromappingdetail
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; }

    public int? ProjectrolesId { get; set; }

    public DateOnly EmployeStartDate { get; set; }

    public DateOnly? EmployeeEndDate { get; set; }

    public bool IsWorking { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedTime { get; set; }

    public string? ModifyBy { get; set; }

    public DateTime? ModifyTime { get; set; }

}
