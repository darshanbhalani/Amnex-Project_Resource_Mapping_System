namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class ProjectDetails
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDepartment { get; set; }
        public string ProjectSkills { get; set; }
        public int NumberOfEmployees { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectStatus { get; set; }

        public int WorkingEmployeeId { get; set; }
        public string Employeename { get; set; }
        public string EmployeeSkills { get; set; }
        public string EmployeeRole { get; set; }
        public DateTime EmployeeStartDate { get; set; }
        public DateTime EmployeeEndDate { get; set; }

        // not allocated employees
        public int employeeId { get; set; }
        public string NotAllocatedEmployeeName { get; set; }
        public string NotAllocatedEmployeeSkills { get; set; }
        public string EmployeeCompanyRole { get; set; }
        //public string employeeProjectRole { get; set; }
        //public string employeeDepartment { get; set; }
        public int EmployeeRating { get; set; }
        //public DateTime aEmployeeStartDate { get; set; }
        //public DateTime aEmployeeEndDate { get; set; }

        // for project role dropdown
        public int ProjectRoleId { get; set; }
        public string ProjectRoleName { get; set; }

        // assign employees
        //public int aEmployeeId { get; set; }
        //public string employeeProjectRole { get; set; }
        //public DateTime EmployeeStartDate { get; set; }
        //public DateTime EmployeeEndDate { get; set; }



    }
}