using Amnex_Project_Resource_Mapping_System.Models;

namespace Amnex_Project_Resource_Mapping_System.Repo.Interfaces
{
    public interface IAccount
    {
        public void login(Login login);
        public void logout();
    }
}
