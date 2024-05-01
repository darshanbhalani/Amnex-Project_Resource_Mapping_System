namespace Amnex_Project_Resource_Mapping_System.Models
{
    public class UserProfileModel
    {
        public Employee ProfileData { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Department> Departments { get; set; }
    }
}
