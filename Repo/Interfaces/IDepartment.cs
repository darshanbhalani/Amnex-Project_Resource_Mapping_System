using Amnex_Project_Resource_Mapping_System.Models;

namespace Amnex_Project_Resource_Mapping_System.Repo.Interfaces
{
    public interface IDepartment
    {
        public void addDepartment(Department data);

        public void removeDepartment(Department data);

        public Department getAllDepartment(Department data);

        public void updateDepartment(Department data);
    }
}
