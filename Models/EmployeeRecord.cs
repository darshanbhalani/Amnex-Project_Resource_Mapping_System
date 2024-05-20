namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class EmployeeRecord
    {
        public string ProjectName { get; set; }
        public string ProjectRoleName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SkillsName { get; set; }
        public int Rating { get; set; }
        public bool IsWorking { get; set; }
        public string EmployeeProjectSkill { get; set; }

    }
}
