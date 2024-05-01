namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class EmployeesModel
    {
        public List<Employee> employees { get; set; }
        public List<Skill> employeeSkills { get; set; }
        public List<Department> employeeDepartments { get; set; }
        public int allocatedEmployees { get; set; }
        public int unallocatedEmployees { get; set; }
    }
}
