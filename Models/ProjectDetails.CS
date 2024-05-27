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
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string NotAllocatedEmployeeName { get; set; }
        public string NotAllocatedEmployeeSkills { get; set; }
        public string EmployeeDesignation { get; set; }

    }
}