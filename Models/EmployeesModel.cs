namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class EmployeesModel
    {
        public List<Employee> Employees { get; set; }
        public List<Skill> EmployeeSkills { get; set; }
        public List<Department> EmployeeDepartments { get; set; }
        public int AllocatedEmployees { get; set; }
        public int UnallocatedEmployees { get; set; }
    }
}
