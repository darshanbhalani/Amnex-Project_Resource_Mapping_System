using Amnex_Project_Resource_Mapping_System.Models;

namespace Amnex_Project_Resource_Mapping_System.Repo.Interfaces
{
    public interface IError
    {
        public void recordError(ErrorModel e);
    }
}
