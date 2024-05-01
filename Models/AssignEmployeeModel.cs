using Microsoft.CodeAnalysis;

namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class AssignEmployeeModel
    {
        public int ProjectId { get; set; }
        public List<int> EmployeesId { get; set; }
        public List<int> ProjectRoleId { get; set; }
        public List<DateOnly?> StartDate { get; set; }
        public List<DateOnly?> EndDate { get; set; }

        // for project role dropdown
        //public int RoleId { get; set; }
        //public string RoleName { get; set; }

    }
}