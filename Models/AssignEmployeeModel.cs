namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class AssignEmployeeModel
    {
        public int ProjectId { get; set; }
        public List<int> EmployeesId { get; set; }
        public List<int> ProjectRoleId { get; set; }
        public List<DateOnly?> StartDate { get; set; }
        public List<DateOnly?> EndDate { get; set; }
    }
}